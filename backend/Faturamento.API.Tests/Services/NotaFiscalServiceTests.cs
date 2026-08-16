using Faturamento.API.Clients;
using Faturamento.API.Clients.DTOs;
using Faturamento.API.Models;
using Faturamento.API.Repositories;
using Faturamento.API.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Faturamento.API.Tests.Services;

public class NotaFiscalServiceTests
{
    private readonly Mock<INotaFiscalRepository> _notaFiscalRepositoryMock;
    private readonly Mock<IEstoqueClient> _estoqueClientMock;
    private readonly NotaFiscalService _service;

    public NotaFiscalServiceTests()
    {
        _notaFiscalRepositoryMock = new Mock<INotaFiscalRepository>();
        _estoqueClientMock = new Mock<IEstoqueClient>();
        _service = new NotaFiscalService(_notaFiscalRepositoryMock.Object, _estoqueClientMock.Object);
    }

    [Fact]
    public async Task GetByNumeracaoAsync_DeveRetornarNotaFiscal_QuandoNumeracaoExistir()
    {
        // Arrange
        var notaEsperada = new NotaFiscal { Id = 1, Numeracao = 1001, Status = StatusNotaFiscal.Aberta };
        _notaFiscalRepositoryMock
            .Setup(r => r.GetByNumeracaoAsync(1001))
            .ReturnsAsync(notaEsperada);

        // Act
        var result = await _service.GetByNumeracaoAsync(1001);

        // Assert
        result.Should().NotBeNull();
        result!.Numeracao.Should().Be(1001);
        _notaFiscalRepositoryMock.Verify(r => r.GetByNumeracaoAsync(1001), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DeveCriarNotaFiscalComStatusAbertaEDataUtc_QuandoDadosForemValidos()
    {
        // Arrange
        var notaInput = new NotaFiscal
        {
            Itens = new List<ItemNotaFiscal>
            {
                new ItemNotaFiscal { CodigoProduto = "PROD-001", DescricaoProduto = "Notebook", Quantidade = 1 }
            }
        };

        _notaFiscalRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal n) => n);

        // Act
        var result = await _service.CreateAsync(notaInput);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(StatusNotaFiscal.Aberta);
        result.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _notaFiscalRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<NotaFiscal>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DeveLancarArgumentException_QuandoNotaNaoPossuirItens()
    {
        // Arrange
        var notaSemItens = new NotaFiscal { Itens = new List<ItemNotaFiscal>() };

        // Act
        var act = async () => await _service.CreateAsync(notaSemItens);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*ao menos um item*");
    }

    [Fact]
    public async Task ImprimirAsync_DeveAbaterEstoqueEmLoteEAtualizarStatusParaFechada_QuandoNotaEstiverAberta()
    {
        // Arrange
        var notaAberta = new NotaFiscal
        {
            Id = 1,
            Numeracao = 1001,
            Status = StatusNotaFiscal.Aberta,
            Itens = new List<ItemNotaFiscal>
            {
                new ItemNotaFiscal { CodigoProduto = "PROD-001", Quantidade = 2 },
                new ItemNotaFiscal { CodigoProduto = "PROD-002", Quantidade = 1 }
            }
        };

        _notaFiscalRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(notaAberta);

        _estoqueClientMock
            .Setup(c => c.AbaterEstoqueLoteAsync(It.IsAny<IEnumerable<ItemAbateEstoqueDto>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ImprimirAsync(1);

        // Assert
        result.Status.Should().Be(StatusNotaFiscal.Fechada);
        _estoqueClientMock.Verify(c => c.AbaterEstoqueLoteAsync(It.Is<IEnumerable<ItemAbateEstoqueDto>>(lote => lote.Count() == 2)), Times.Once);
        _notaFiscalRepositoryMock.Verify(r => r.UpdateAsync(notaAberta), Times.Once);
    }

    [Fact]
    public async Task ImprimirAsync_DeveLancarInvalidOperationException_QuandoNotaJaEstiverFechada()
    {
        // Arrange
        var notaFechada = new NotaFiscal
        {
            Id = 1,
            Numeracao = 1001,
            Status = StatusNotaFiscal.Fechada,
            Itens = new List<ItemNotaFiscal>
            {
                new ItemNotaFiscal { CodigoProduto = "PROD-001", Quantidade = 1 }
            }
        };

        _notaFiscalRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(notaFechada);

        // Act
        var act = async () => await _service.ImprimirAsync(1);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*já está com status 'Fechada'*");

        _estoqueClientMock.Verify(c => c.AbaterEstoqueLoteAsync(It.IsAny<IEnumerable<ItemAbateEstoqueDto>>()), Times.Never);
        _notaFiscalRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<NotaFiscal>()), Times.Never);
    }

    [Fact]
    public async Task ImprimirAsync_DeveManterNotaAbertaEPropagarExcecao_QuandoAbateEmLoteFalhar()
    {
        // Arrange
        var notaAberta = new NotaFiscal
        {
            Id = 1,
            Numeracao = 1001,
            Status = StatusNotaFiscal.Aberta,
            Itens = new List<ItemNotaFiscal>
            {
                new ItemNotaFiscal { CodigoProduto = "PROD-001", Quantidade = 2 }
            }
        };

        _notaFiscalRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(notaAberta);

        _estoqueClientMock
            .Setup(c => c.AbaterEstoqueLoteAsync(It.IsAny<IEnumerable<ItemAbateEstoqueDto>>()))
            .ThrowsAsync(new InvalidOperationException("Serviço de estoque indisponível"));

        // Act
        var act = async () => await _service.ImprimirAsync(1);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*estoque indisponível*");

        notaAberta.Status.Should().Be(StatusNotaFiscal.Aberta);
        _notaFiscalRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<NotaFiscal>()), Times.Never);
    }

    [Fact]
    public async Task ImprimirAsync_DeveDispararEstornoDeEstoque_QuandoUpdateDoBancoFaturamentoFalhar()
    {
        // Arrange
        var notaAberta = new NotaFiscal
        {
            Id = 1,
            Numeracao = 1001,
            Status = StatusNotaFiscal.Aberta,
            Itens = new List<ItemNotaFiscal>
            {
                new ItemNotaFiscal { CodigoProduto = "PROD-001", Quantidade = 2 }
            }
        };

        _notaFiscalRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(notaAberta);

        _estoqueClientMock
            .Setup(c => c.AbaterEstoqueLoteAsync(It.IsAny<IEnumerable<ItemAbateEstoqueDto>>()))
            .Returns(Task.CompletedTask);

        // Simula falha de persistência no banco do Faturamento no momento da gravação do status Fechada
        _notaFiscalRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<NotaFiscal>()))
            .ThrowsAsync(new Exception("Falha de gravação no banco PostgreSQL faturamento_db"));

        _estoqueClientMock
            .Setup(c => c.EstornarEstoqueLoteAsync(It.IsAny<IEnumerable<ItemAbateEstoqueDto>>()))
            .Returns(Task.CompletedTask);

        // Act
        var act = async () => await _service.ImprimirAsync(1);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*revertido com sucesso*");

        // Valida que o abate foi feito primeiro E em seguida a Ação Compensatória de estorno foi acionada
        _estoqueClientMock.Verify(c => c.AbaterEstoqueLoteAsync(It.IsAny<IEnumerable<ItemAbateEstoqueDto>>()), Times.Once);
        _estoqueClientMock.Verify(c => c.EstornarEstoqueLoteAsync(It.IsAny<IEnumerable<ItemAbateEstoqueDto>>()), Times.Once);
    }
}
