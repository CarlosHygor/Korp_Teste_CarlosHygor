using System.Net;
using System.Net.Http.Json;
using Estoque.API.Data;
using Estoque.API.DTOs;
using Estoque.API.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Estoque.API.Tests.Controllers;

public class ProdutosControllerE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProdutosControllerE2ETests(WebApplicationFactory<Program> factory)
    {
        // Customiza a WebApplicationFactory para usar Banco de Dados InMemory durante o teste E2E
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove a configuração do DbContext do PostgreSQL
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<EstoqueDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona o DbContext usando InMemoryDatabase com nome único por teste
                services.AddDbContext<EstoqueDbContext>(options =>
                {
                    options.UseInMemoryDatabase("EstoqueApiE2ETestDb");
                });
            });
        });
    }

    private async Task SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
        
        if (!await db.Produtos.AnyAsync(p => p.Codigo == "E2E-PROD-01"))
        {
            db.Produtos.Add(new Produto
            {
                Codigo = "E2E-PROD-01",
                Descricao = "Teclado E2E Teste",
                Saldo = 10
            });
            await db.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task AbaterEstoque_QuandoSaldoForSuficiente_DeveRetornarStatus200OK()
    {
        // Arrange
        await SeedDatabaseAsync();
        var client = _factory.CreateClient();
        var requestDto = new AbaterEstoqueDto(Quantidade: 3);

        // Act (Dispara requisição HTTP POST real na pipeline da API)
        var response = await client.PostAsJsonAsync("/api/produtos/E2E-PROD-01/abater", requestDto);

        // Assert (Valida o Status HTTP 200 OK da API)
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AbaterEstoque_QuandoSaldoForInsuficiente_DeveRetornarStatus422UnprocessableEntity()
    {
        // Arrange
        await SeedDatabaseAsync();
        var client = _factory.CreateClient();
        var requestDto = new AbaterEstoqueDto(Quantidade: 50);

        // Act
        var response = await client.PostAsJsonAsync("/api/produtos/E2E-PROD-01/abater", requestDto);

        // Assert (Valida o mapeamento para HTTP 422 Unprocessable Entity)
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task AbaterEstoque_QuandoQuantidadeForInvalida_DeveRetornarStatus400BadRequest()
    {
        // Arrange
        await SeedDatabaseAsync();
        var client = _factory.CreateClient();
        var requestDto = new AbaterEstoqueDto(Quantidade: 0); // Data Annotations exige >= 1

        // Act
        var response = await client.PostAsJsonAsync("/api/produtos/E2E-PROD-01/abater", requestDto);

        // Assert (Valida bloqueio automático do ASP.NET Core via Model Validation)
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
