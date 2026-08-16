using System.Net;
using Faturamento.API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Faturamento.API.Middleware;

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
        _logger.LogError(exception, "Ocorreu uma exceção tratada pelo GlobalExceptionHandler no Faturamento: {Message}", exception.Message);

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
            NotaFiscalNaoEncontradaException ex => (
                StatusCodes.Status404NotFound,
                new { mensagem = ex.Message }
            ),
            KeyNotFoundException ex => (
                StatusCodes.Status404NotFound,
                new { mensagem = ex.Message }
            ),
            NotaFiscalStatusInvalidoException ex => (
                StatusCodes.Status400BadRequest,
                new { mensagem = ex.Message }
            ),
            InvalidOperationException ex => (
                StatusCodes.Status400BadRequest,
                new { mensagem = ex.Message }
            ),
            ServicoEstoqueIndisponivelException ex => (
                StatusCodes.Status503ServiceUnavailable,
                new { mensagem = ex.Message }
            ),
            HttpRequestException ex => (
                StatusCodes.Status503ServiceUnavailable,
                new { mensagem = ex.Message }
            ),
            ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                new { mensagem = ex.Message }
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                new { mensagem = "Ocorreu um erro interno inesperado no servidor de Faturamento." }
            )
        };
    }
}
