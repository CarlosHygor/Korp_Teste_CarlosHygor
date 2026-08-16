using Faturamento.API.Clients;
using Faturamento.API.Data;
using Faturamento.API.Repositories;
using Faturamento.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração de CORS para liberar chamadas do frontend Angular (porta 4200)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuração do DbContext do Entity Framework Core com PostgreSQL (faturamento_db)
var connectionString = builder.Configuration.GetConnectionString("FaturamentoConnection");
builder.Services.AddDbContext<FaturamentoDbContext>(options =>
    options.UseNpgsql(connectionString));

// Cliente HTTP para integração com Estoque.API (WebClient / FeignClient no Spring)
builder.Services.AddHttpClient<IEstoqueClient, EstoqueClient>(client =>
{
    var estoqueUrl = builder.Configuration["Services:EstoqueUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(estoqueUrl.EndsWith("/") ? estoqueUrl : $"{estoqueUrl}/");
});

// Injeção de Dependência dos Repositórios (Scoped ~ @RequestScope / Spring Bean Scoped)
builder.Services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
builder.Services.AddScoped<IItemNotaFiscalRepository, ItemNotaFiscalRepository>();

// Injeção de Dependência da Camada de Serviços
builder.Services.AddScoped<IItemNotaFiscalService, ItemNotaFiscalService>();
builder.Services.AddScoped<INotaFiscalService, NotaFiscalService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Garantir que a base de dados faturamento_db seja recriada no PostgreSQL com o esquema correto e popular Seed Data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();
    await DbInitializer.SeedAsync(dbContext);
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Faturamento API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAngular");

app.MapControllers();

app.MapGet("/api/faturamento/ping", () => new
{
    servico = "Faturamento.API",
    status = "ok",
    horario = DateTime.UtcNow
});

app.Run();

// Habilita a classe Program visível para testes de integração com WebApplicationFactory
public partial class Program { }
