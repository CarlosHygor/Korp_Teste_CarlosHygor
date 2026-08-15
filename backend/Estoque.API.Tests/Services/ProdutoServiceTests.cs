using Estoque.API.Exceptions;
using Estoque.API.Models;
using Estoque.API.Repositories;
using Estoque.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Estoque.API.Tests.Services;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly ProdutoService _produtoService;

    public ProdutoServiceTests()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _produtoService = new ProdutoService(_produtoRepositoryMock.Object);
    }

    #region Testes de Abate de Estoque (AbaterEstoqueAsync)

    [Fact]
    public async Task AbaterEstoqueAsync_QuandoSaldoForSuficiente_DeveAbaterSaldoEAtualizarNoRepositorio()
    {
        // Arrange
        var codigo = "PROD-001";
        var produtoOriginal = new Produto { Id = 1, Codigo = codigo, Descricao = "Teclado", Saldo = 10 };

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
        var produtoOriginal = new Produto { Id = 1, Codigo = codigo, Descricao = "Teclado", Saldo = 5 };

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
        await act.Should().ThrowAsync<KeyNotFoundException>()
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

    #endregion

    #region Testes de Cadastro (CreateAsync)

    [Fact]
    public async Task CreateAsync_QuandoDadosForemValidos_DeveAdicionarEGrafarProduto()
    {
        // Arrange
        var novoProduto = new Produto { Codigo = "PROD-100", Descricao = "Mouse Gamer", Saldo = 15 };
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
}
