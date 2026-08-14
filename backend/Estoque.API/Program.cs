using Estoque.API.Data;
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Garantir que a base de dados estoque_db seja criada no PostgreSQL
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Estoque API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAngular");

app.MapGet("/api/estoque/ping", () => new
{
    servico = "Estoque.API",
    status = "ok",
    horario = DateTime.UtcNow
});

app.Run();
