using Estoque.API.Builders;
using Estoque.API.Data;
using Estoque.API.DTOs;
using Estoque.API.Exceptions;
using Estoque.API.Models;
using Estoque.API.Repositories;
using Estoque.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;

namespace Estoque.API.Tests.Services;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly EstoqueDbContext _context;
    private readonly ProdutoService _produtoService;

    public ProdutoServiceTests()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();

        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new EstoqueDbContext(options);
        _produtoService = new ProdutoService(_produtoRepositoryMock.Object, _context);
    }

    #region Testes de Abate de Estoque (AbaterEstoqueAsync)

    [Fact]
    public async Task AbaterEstoqueAsync_QuandoSaldoForSuficiente_DeveAbaterSaldoEAtualizarNoRepositorio()
    {
        // Arrange
        var codigo = "PROD-001";
        var produtoOriginal = new ProdutoBuilder()
            .ComId(1)
            .ComCodigo(codigo)
            .ComDescricao("Teclado")
            .ComSaldo(10)
            .Build();

        _produtoRepositoryMock.Setup(r => r.GetByCodigoAsync(codigo))
                             .ReturnsAsync(produtoOriginal);

        // Act
        await _produtoService.AbaterEstoqueAsync(codigo, 2);

        // Assert
        produtoOriginal.Saldo.Should().Be(8);
        _produtoRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Produto>(p => p.Saldo == 8)), Times.Once);
    }

    [Fact]
    public async Task AbaterEstoqueAsync_QuandoSaldoForInsuficiente_DeveLancarEstoqueInsuficienteExceptionENaoAlterarSaldo()
    {
        // Arrange
        var codigo = "PROD-001";
        var produtoOriginal = new ProdutoBuilder()
            .ComId(1)
            .ComCodigo(codigo)
            .ComDescricao("Teclado")
            .ComSaldo(5)
            .Build();

        _produtoRepositoryMock.Setup(r => r.GetByCodigoAsync(codigo))
                             .ReturnsAsync(produtoOriginal);

        // Act
        Func<Task> act = async () => await _produtoService.AbaterEstoqueAsync(codigo, 10);

        // Assert
        await act.Should().ThrowAsync<EstoqueInsuficienteException>()
                 .WithMessage("*Estoque insuficiente*");

        produtoOriginal.Saldo.Should().Be(5); // Garante que o saldo não foi alterado
        _produtoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task AbaterEstoqueAsync_QuandoProdutoNaoExistir_DeveLancarKeyNotFoundException()
    {
        // Arrange
        var codigoInexistente = "PROD-999";
        _produtoRepositoryMock.Setup(r => r.GetByCodigoAsync(codigoInexistente))
                             .ReturnsAsync((Produto?)null);

        // Act
        Func<Task> act = async () => await _produtoService.AbaterEstoqueAsync(codigoInexistente, 1);

        // Assert
        await act.Should().ThrowAsync<ProdutoNaoEncontradoException>()
                 .WithMessage("*não encontrado no estoque*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task AbaterEstoqueAsync_QuandoQuantidadeForInvalida_DeveLancarArgumentException(int quantidadeInvalida)
    {
        // Act
        Func<Task> act = async () => await _produtoService.AbaterEstoqueAsync("PROD-001", quantidadeInvalida);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("*deve ser maior que zero*");
    }

    [Fact]
    public async Task AbaterEstoqueLoteAsync_DeveAbaterTodosOsProdutos_QuandoTodosForemValidos()
    {
        // Arrange
        var prod1 = new ProdutoBuilder().ComId(1).ComCodigo("PROD-001").ComDescricao("Teclado").ComSaldo(10).Build();
        var prod2 = new ProdutoBuilder().ComId(2).ComCodigo("PROD-002").ComDescricao("Mouse").ComSaldo(5).Build();

        _produtoRepositoryMock.Setup(r => r.GetByCodigoAsync("PROD-001")).ReturnsAsync(prod1);
        _produtoRepositoryMock.Setup(r => r.GetByCodigoAsync("PROD-002")).ReturnsAsync(prod2);

        var lote = new List<AbaterItemEstoqueDto>
        {
            new AbaterItemEstoqueDto("PROD-001", 2),
            new AbaterItemEstoqueDto("PROD-002", 1)
        };

        // Act
        await _produtoService.AbaterEstoqueLoteAsync(lote);

        // Assert
        prod1.Saldo.Should().Be(8);
        prod2.Saldo.Should().Be(4);
        _produtoRepositoryMock.Verify(r => r.UpdateAsync(prod1), Times.Once);
        _produtoRepositoryMock.Verify(r => r.UpdateAsync(prod2), Times.Once);
    }

    [Fact]
    public async Task EstornarEstoqueAsync_DeveRestabelecerSaldo_QuandoQuantidadeForValida()
    {
        // Arrange
        var prod = new ProdutoBuilder().ComId(1).ComCodigo("PROD-001").ComDescricao("Teclado").ComSaldo(8).Build();
        _produtoRepositoryMock.Setup(r => r.GetByCodigoAsync("PROD-001")).ReturnsAsync(prod);

        // Act
        await _produtoService.EstornarEstoqueAsync("PROD-001", 2);

        // Assert
        prod.Saldo.Should().Be(10);
        _produtoRepositoryMock.Verify(r => r.UpdateAsync(prod), Times.Once);
    }

    #endregion

    #region Testes de Cadastro (CreateAsync)

    [Fact]
    public async Task CreateAsync_QuandoDadosForemValidos_DeveAdicionarEGrafarProduto()
    {
        // Arrange
        var novoProduto = new ProdutoBuilder().ComCodigo("PROD-100").ComDescricao("Mouse Gamer").ComSaldo(15).Build();
        _produtoRepositoryMock.Setup(r => r.AddAsync(novoProduto))
                             .ReturnsAsync(novoProduto);

        // Act
        var produtoCriado = await _produtoService.CreateAsync(novoProduto);

        // Assert
        produtoCriado.Should().NotBeNull();
        produtoCriado.Codigo.Should().Be("PROD-100");
        _produtoRepositoryMock.Verify(r => r.AddAsync(novoProduto), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_QuandoBancoLancarDbUpdateException_DeveTraduzirParaCodigoProdutoDuplicadoException()
    {
        // Arrange
        var produtoDuplicado = new Produto { Codigo = "PROD-001", Descricao = "Mouse", Saldo = 10 };
        _produtoRepositoryMock.Setup(r => r.AddAsync(produtoDuplicado))
                             .ThrowsAsync(new DbUpdateException("Violação de restrição única"));

        // Act
        Func<Task> act = async () => await _produtoService.CreateAsync(produtoDuplicado);

        // Assert
        await act.Should().ThrowAsync<CodigoProdutoDuplicadoException>()
                 .WithMessage("*Já existe um produto cadastrado com o código 'PROD-001'*");
    }

    [Fact]
    public async Task CreateAsync_QuandoSaldoForNegativo_DeveLancarArgumentException()
    {
        // Arrange
        var produtoInvalido = new Produto { Codigo = "PROD-002", Descricao = "Monitor", Saldo = -10 };

        // Act
        Func<Task> act = async () => await _produtoService.CreateAsync(produtoInvalido);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("*não pode ser negativo*");

        _produtoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Produto>()), Times.Never);
    }

    #endregion

    #region Testes de Paginação (GetPaginatedAsync)

    [Fact]
    public async Task GetPaginatedAsync_DeveRetornarResultadoPaginadoEPropriedadesCalculadas()
    {
        // Arrange
        var produtos = new List<Produto>
        {
            new ProdutoBuilder().ComId(1).ComCodigo("PROD-01").ComDescricao("Item 1").ComSaldo(10).Build(),
            new ProdutoBuilder().ComId(2).ComCodigo("PROD-02").ComDescricao("Item 2").ComSaldo(5).Build()
        };

        _produtoRepositoryMock.Setup(r => r.GetPaginatedAsync(1, 10))
                             .ReturnsAsync((produtos, 25));

        // Act
        var result = await _produtoService.GetPaginatedAsync(1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Itens.Count().Should().Be(2);
        result.PaginaAtual.Should().Be(1);
        result.TamanhoPagina.Should().Be(10);
        result.TotalRegistros.Should().Be(25);
        result.TotalPaginas.Should().Be(3);
        result.TemPaginaAnterior.Should().BeFalse();
        result.TemProximaPagina.Should().BeTrue();
    }

    #endregion
}
