using Faturamento.API.Clients;
using Faturamento.API.Data;
using Faturamento.API.Middleware;
using Faturamento.API.Repositories;
using Faturamento.API.Services;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.API.Configuration;

public static class DependencyInjectionSetup
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuração do DbContext do Entity Framework Core com PostgreSQL (faturamento_db)
        var connectionString = configuration.GetConnectionString("FaturamentoConnection");
        services.AddDbContext<FaturamentoDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Cliente HTTP para integração distribuída com Estoque.API (WebClient / FeignClient no Spring)
        services.AddHttpClient<IEstoqueClient, EstoqueClient>(client =>
        {
            var estoqueUrl = configuration["Services:EstoqueUrl"] ?? "http://localhost:5000";
            client.BaseAddress = new Uri(estoqueUrl.EndsWith("/") ? estoqueUrl : $"{estoqueUrl}/");
        });

        // Injeção de Dependência dos Repositórios (Scoped ~ @RequestScope / Spring Bean Scoped)
        services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
        services.AddScoped<IItemNotaFiscalRepository, ItemNotaFiscalRepository>();

        // Injeção de Dependência da Camada de Serviços
        services.AddScoped<IItemNotaFiscalService, ItemNotaFiscalService>();
        services.AddScoped<INotaFiscalService, NotaFiscalService>();

        // Registra o Manipulador Global de Exceções (.NET 8 IExceptionHandler)
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // Registra o ASP.NET Core Health Checks nativo
        services.AddHealthChecks();

        return services;
    }
}
