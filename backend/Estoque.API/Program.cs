using Estoque.API.Data;
using Estoque.API.Repositories;
using Estoque.API.Services;
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

// Configuração do DbContext do Entity Framework Core com PostgreSQL (estoque_db)
var connectionString = builder.Configuration.GetConnectionString("EstoqueConnection");
builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseNpgsql(connectionString));

// Injeção de Dependência (DI) - Escopo por Requisição (AddScoped <-> @RequestScope / Spring Beans padrao em requisições)
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Garantir que a base de dados estoque_db seja criada no PostgreSQL e adionica dados fícticios via data.sql
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    await DbInitializer.SeedAsync(dbContext);
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Estoque API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAngular");

app.MapControllers();

app.MapGet("/api/estoque/ping", () => new
{
    servico = "Estoque.API",
    status = "ok",
    horario = DateTime.UtcNow
});

app.Run();

// Habilita a classe Program visível para testes de integração com WebApplicationFactory
public partial class Program { }
