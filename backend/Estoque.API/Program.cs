using Estoque.API.Configuration;
using Estoque.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuração modular de serviços via Extension Methods (.NET 8 Clean Architecture)
builder.Services
    .AddCorsConfiguration()
    .AddApplicationServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ativa o middleware global de exceções
app.UseExceptionHandler();

// Inicialização e Seed da base de dados PostgreSQL estoque_db
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

// Health Check nativo do ASP.NET Core (/health)
app.MapHealthChecks("/health");

app.Run();

// Habilita a classe Program visível para testes de integração com WebApplicationFactory
public partial class Program { }
