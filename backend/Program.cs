var builder = WebApplication.CreateBuilder(args);

// Configuração do CORS (libera chamadas vindas do Angular na porta 4200)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuração do Swagger para testes e documentação dos endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilitar Swagger em todos os ambientes (Desenvolvimento / Container)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API C# v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAngular");

// Endpoint mínimo de teste (Ping / Health Check)
app.MapGet("/api/ping", () => new 
{ 
    status = "ok", 
    mensagem = "API C# ASP.NET Core 8 rodando com sucesso!",
    horario = DateTime.UtcNow 
})
.WithName("GetPing")
.WithOpenApi();

app.Run();
