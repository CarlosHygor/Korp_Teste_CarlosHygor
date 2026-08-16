using Faturamento.API.Data;
using Faturamento.API.Models;
using Faturamento.API.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Faturamento.API.Tests.Repositories;

public class NotaFiscalRepositoryTests
{
    private FaturamentoDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<FaturamentoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new FaturamentoDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_DevePersistirNotaComStatusInicialAbertaEItensRelacionados()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var repository = new NotaFiscalRepository(context);

        var novaNota = new NotaFiscal
        {
            Status = StatusNotaFiscal.Aberta,
            DataCriacao = DateTime.UtcNow,
            Itens = new List<ItemNotaFiscal>
            {
                new ItemNotaFiscal { CodigoProduto = "CAD-001", DescricaoProduto = "Caderno", Quantidade = 2 }
            }
        };

        // Act
        var result = await repository.CreateAsync(novaNota);

        // Assert - Valida que a nota foi persistida com ID gerado, status Aberta e itens associados
        result.Id.Should().BeGreaterThan(0);
        result.Status.Should().Be(StatusNotaFiscal.Aberta);
        result.Itens.Should().HaveCount(1);
        result.Itens.First().CodigoProduto.Should().Be("CAD-001");

        // Validação direta no banco (DbContext)
        var dbNota = await context.NotasFiscais.Include(n => n.Itens).FirstOrDefaultAsync(n => n.Id == result.Id);
        dbNota.Should().NotBeNull();
        dbNota!.Status.Should().Be(StatusNotaFiscal.Aberta);
    }
}
