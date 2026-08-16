using Estoque.API.Data;
using Estoque.API.Middleware;
using Estoque.API.Repositories;
using Estoque.API.Services;
using Microsoft.EntityFrameworkCore;

namespace Estoque.API.Configuration;

public static class DependencyInjectionSetup
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuração do DbContext do Entity Framework Core com PostgreSQL (estoque_db)
        var connectionString = configuration.GetConnectionString("EstoqueConnection");
        services.AddDbContext<EstoqueDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Injeção de Dependência (DI) - Repositórios e Serviços (Scoped ~ @RequestScope / Spring Beans padrão)
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IProdutoService, ProdutoService>();

        // Registra o Manipulador Global de Exceções (.NET 8 IExceptionHandler)
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // Registra o ASP.NET Core Health Checks nativo
        services.AddHealthChecks();

        return services;
    }
}
