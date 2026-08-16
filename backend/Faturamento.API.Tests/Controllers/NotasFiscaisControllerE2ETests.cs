using System.Net;
using System.Net.Http.Json;
using Faturamento.API.Clients;
using Faturamento.API.Data;
using Faturamento.API.DTOs;
using Faturamento.API.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Faturamento.API.Tests.Controllers;

public class NotasFiscaisControllerE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IEstoqueClient> _estoqueClientMock = new();

    public NotasFiscaisControllerE2ETests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Substitui o DbContext do PostgreSQL por InMemoryDatabase único para o teste E2E
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<FaturamentoDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<FaturamentoDbContext>(options =>
                {
                    options.UseInMemoryDatabase("FaturamentoApiE2ETestDb");
                });

                // Substitui o IEstoqueClient por um Mock para simular respostas do Estoque.API sem subir a outra API
                var clientDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEstoqueClient));
                if (clientDescriptor != null)
                {
                    services.Remove(clientDescriptor);
                }

                services.AddSingleton(_estoqueClientMock.Object);
            });
        });
    }

    private async Task SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();

        if (!await db.NotasFiscais.AnyAsync(n => n.Numeracao == 9999))
        {
            db.NotasFiscais.Add(new NotaFiscal
            {
                Numeracao = 9999,
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = DateTime.UtcNow,
                Itens = new List<ItemNotaFiscal>
                {
                    new ItemNotaFiscal { CodigoProduto = "PROD-E2E-1", DescricaoProduto = "Produto E2E", Quantidade = 2 }
                }
            });
            await db.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task GetAll_DeveRetornarStatus200OKEListaDeNotas()
    {
        // Arrange
        await SeedDatabaseAsync();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/notasfiscais");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var resultadoPaginado = await response.Content.ReadFromJsonAsync<PagedResultDto<NotaFiscalResponseDto>>();
        resultadoPaginado.Should().NotBeNull();
        resultadoPaginado!.Itens.Should().NotBeEmpty();
        resultadoPaginado.TotalRegistros.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetByNumeracao_QuandoNumeracaoExistir_DeveRetornarStatus200OKEPayloadValido()
    {
        // Arrange
        await SeedDatabaseAsync();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/notasfiscais/numeracao/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var nota = await response.Content.ReadFromJsonAsync<NotaFiscalResponseDto>();
        nota.Should().NotBeNull();
        nota!.Numeracao.Should().Be(9999);
        nota.Status.Should().Be("Aberta");
    }

    [Fact]
    public async Task Create_QuandoPayloadForValido_DeveRetornarStatus201Created()
    {
        // Arrange
        var client = _factory.CreateClient();
        var dto = new CreateNotaFiscalDto(
            Itens: new List<CreateItemNotaFiscalDto>
            {
                new CreateItemNotaFiscalDto("PROD-E2E-NEW", "Produto Novo", Quantidade: 1)
            }
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/notasfiscais", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var notaCriada = await response.Content.ReadFromJsonAsync<NotaFiscalResponseDto>();
        notaCriada.Should().NotBeNull();
        notaCriada!.Status.Should().Be("Aberta");
    }

    [Fact]
    public async Task Create_QuandoListaDeItensForVazia_DeveRetornarStatus400BadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var dto = new CreateNotaFiscalDto(Itens: new List<CreateItemNotaFiscalDto>());

        // Act
        var response = await client.PostAsJsonAsync("/api/notasfiscais", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Imprimir_QuandoNotaEstiverAbertaEEstoqueComSucesso_DeveRetornarStatus200OKENotaFechada()
    {
        // Arrange
        await SeedDatabaseAsync();
        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();
        var notaDb = await db.NotasFiscais.FirstAsync(n => n.Numeracao == 9999);

        _estoqueClientMock
            .Setup(c => c.AbaterEstoqueLoteAsync(It.IsAny<IEnumerable<Faturamento.API.Clients.DTOs.ItemAbateEstoqueDto>>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await client.PostAsync($"/api/notasfiscais/{notaDb.Id}/imprimir", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var notaImpressa = await response.Content.ReadFromJsonAsync<NotaFiscalResponseDto>();
        notaImpressa.Should().NotBeNull();
        notaImpressa!.Status.Should().Be("Fechada");
    }
}
