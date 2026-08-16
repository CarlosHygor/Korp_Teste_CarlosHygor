using Estoque.API.Data;
using Estoque.API.Models;
using Estoque.API.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Estoque.API.Tests.Repositories;

public class ProdutoRepositoryIntegrationTests
{
    private EstoqueDbContext CriarDbContextInMemory()
    {
        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new EstoqueDbContext(options);
    }

    [Fact]
    public async Task AddAsync_E_GetByCodigoAsync_DeveSalvarEBuscarProdutoRealNoDbContext()
    {
        // Arrange
        using var context = CriarDbContextInMemory();
        var repository = new ProdutoRepository(context);
        var produto = new Produto { Codigo = "PROD-INT-01", Descricao = "Cadeira Ergonômica", Saldo = 20 };

        // Act
        await repository.AddAsync(produto);
        var produtoConsultado = await repository.GetByCodigoAsync("PROD-INT-01");

        // Assert
        produtoConsultado.Should().NotBeNull();
        produtoConsultado!.Codigo.Should().Be("PROD-INT-01");
        produtoConsultado.Saldo.Should().Be(20);
    }

    [Fact]
    public async Task UpdateAsync_DeveAtualizarProdutoNoDbContextInMemory()
    {
        // Arrange
        using var context = CriarDbContextInMemory();
        var repository = new ProdutoRepository(context);
        var produto = new Produto { Codigo = "PROD-INT-02", Descricao = "Mesa de Escritório", Saldo = 15 };

        await repository.AddAsync(produto);

        // Act
        produto.Saldo = 10;
        await repository.UpdateAsync(produto);

        var produtoAtualizado = await repository.GetByCodigoAsync("PROD-INT-02");

        // Assert
        produtoAtualizado.Should().NotBeNull();
        produtoAtualizado!.Saldo.Should().Be(10);
    }
}
