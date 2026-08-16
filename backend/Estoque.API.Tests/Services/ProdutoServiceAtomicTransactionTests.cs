using Estoque.API.Data;
using Estoque.API.DTOs;
using Estoque.API.Exceptions;
using Estoque.API.Models;
using Estoque.API.Repositories;
using Estoque.API.Services;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Estoque.API.Tests.Services;

public class ProdutoServiceAtomicTransactionTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly EstoqueDbContext _context;
    private readonly IProdutoRepository _produtoRepository;
    private readonly ProdutoService _produtoService;

    public ProdutoServiceAtomicTransactionTests()
    {
        // Cria uma conexão SQLite em memória dedicada que suporta transações relacionais atômicas reais (BEGIN TRANSACTION, ROLLBACK, COMMIT)
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new EstoqueDbContext(options);
        _context.Database.EnsureCreated();

        _produtoRepository = new ProdutoRepository(_context);
        _produtoService = new ProdutoService(_produtoRepository, _context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    [Fact]
    public async Task AbaterEstoqueLoteAsync_QuandoUmItemDaListaForInexistente_DeveRealizarRollbackAtomicamenteDeTodosOsItens()
    {
        // Arrange (Cadastra 2 produtos com saldo 10 no banco real SQLite)
        _context.Produtos.AddRange(
            new Produto { Codigo = "ATOMIC-01", Descricao = "Produto 1", Saldo = 10 },
            new Produto { Codigo = "ATOMIC-02", Descricao = "Produto 2", Saldo = 10 }
        );
        await _context.SaveChangesAsync();

        var loteComFalha = new List<AbaterItemEstoqueDto>
        {
            new AbaterItemEstoqueDto("ATOMIC-01", 5), // Válido (restaria 5)
            new AbaterItemEstoqueDto("ATOMIC-02", 5), // Válido (restaria 5)
            new AbaterItemEstoqueDto("PROD-INEXISTENTE", 1) // Inválido (vai falhar)
        };

        // Act
        Func<Task> act = async () => await _produtoService.AbaterEstoqueLoteAsync(loteComFalha);

        // Assert
        await act.Should().ThrowAsync<ProdutoNaoEncontradoException>();

        // Limpa o ChangeTracker do EF Core em memória para consultar o estado real revertido do banco SQLite
        _context.ChangeTracker.Clear();

        // Valida que o Rollback atômico da transação impediu o abate dos produtos 1 e 2!
        var prod1 = await _context.Produtos.FirstAsync(p => p.Codigo == "ATOMIC-01");
        var prod2 = await _context.Produtos.FirstAsync(p => p.Codigo == "ATOMIC-02");

        prod1.Saldo.Should().Be(10, "o saldo do Produto 1 não deve ser alterado após o Rollback da transação.");
        prod2.Saldo.Should().Be(10, "o saldo do Produto 2 não deve ser alterado após o Rollback da transação.");
    }

    [Fact]
    public async Task AbaterEstoqueLoteAsync_QuandoUmItemTiverEstoqueInsuficiente_DeveRealizarRollbackAtomicamenteDeTodosOsItens()
    {
        // Arrange
        _context.Produtos.AddRange(
            new Produto { Codigo = "ATOMIC-03", Descricao = "Produto 3", Saldo = 10 },
            new Produto { Codigo = "ATOMIC-04", Descricao = "Produto 4", Saldo = 2 }
        );
        await _context.SaveChangesAsync();

        var loteComEstoqueInsuficiente = new List<AbaterItemEstoqueDto>
        {
            new AbaterItemEstoqueDto("ATOMIC-03", 5), // Válido
            new AbaterItemEstoqueDto("ATOMIC-04", 10) // Falhará por EstoqueInsuficiente (2 < 10)
        };

        // Act
        Func<Task> act = async () => await _produtoService.AbaterEstoqueLoteAsync(loteComEstoqueInsuficiente);

        // Assert
        await act.Should().ThrowAsync<EstoqueInsuficienteException>();

        // Limpa o ChangeTracker do EF Core em memória para consultar o estado real revertido do banco SQLite
        _context.ChangeTracker.Clear();

        // Valida a atomicidade: nenhum dos dois produtos teve o saldo alterado no banco
        var prod3 = await _context.Produtos.FirstAsync(p => p.Codigo == "ATOMIC-03");
        var prod4 = await _context.Produtos.FirstAsync(p => p.Codigo == "ATOMIC-04");

        prod3.Saldo.Should().Be(10);
        prod4.Saldo.Should().Be(2);
    }

    [Fact]
    public async Task EstornarEstoqueLoteAsync_DeveRestabelecerSaldoDeTodosOsProdutosAtomicamente()
    {
        // Arrange
        _context.Produtos.AddRange(
            new Produto { Codigo = "ATOMIC-05", Descricao = "Produto 5", Saldo = 5 },
            new Produto { Codigo = "ATOMIC-06", Descricao = "Produto 6", Saldo = 3 }
        );
        await _context.SaveChangesAsync();

        var loteEstorno = new List<AbaterItemEstoqueDto>
        {
            new AbaterItemEstoqueDto("ATOMIC-05", 5),
            new AbaterItemEstoqueDto("ATOMIC-06", 3)
        };

        // Act
        await _produtoService.EstornarEstoqueLoteAsync(loteEstorno);

        // Assert
        var prod5 = await _context.Produtos.FirstAsync(p => p.Codigo == "ATOMIC-05");
        var prod6 = await _context.Produtos.FirstAsync(p => p.Codigo == "ATOMIC-06");

        prod5.Saldo.Should().Be(10);
        prod6.Saldo.Should().Be(6);
    }
}
