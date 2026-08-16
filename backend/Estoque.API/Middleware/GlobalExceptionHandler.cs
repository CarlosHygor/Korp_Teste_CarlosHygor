using System.Net;
using Estoque.API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Estoque.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Ocorreu uma exceção tratada pelo GlobalExceptionHandler: {Message}", exception.Message);

        var (statusCode, responseBody) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(responseBody, cancellationToken);

        return true;
    }

    private static (int StatusCode, object ResponseBody) MapException(Exception exception)
    {
        return exception switch
        {
            ProdutoNaoEncontradoException ex => (
                StatusCodes.Status404NotFound,
                new { mensagem = ex.Message }
            ),
            KeyNotFoundException ex => (
                StatusCodes.Status404NotFound,
                new { mensagem = ex.Message }
            ),
            CodigoProdutoDuplicadoException ex => (
                StatusCodes.Status409Conflict,
                new { mensagem = ex.Message }
            ),
            EstoqueInsuficienteException ex => (
                StatusCodes.Status422UnprocessableEntity,
                new
                {
                    mensagem = ex.Message,
                    codigoProduto = ex.CodigoProduto,
                    saldoAtual = ex.SaldoAtual,
                    quantidadeSolicitada = ex.QuantidadeSolicitada
                }
            ),
            ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                new { mensagem = ex.Message }
            ),
            InvalidOperationException ex => (
                StatusCodes.Status400BadRequest,
                new { mensagem = ex.Message }
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                new { mensagem = "Ocorreu um erro interno inesperado no servidor de Estoque." }
            )
        };
    }
}
