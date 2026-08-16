namespace Faturamento.API.Exceptions;

public class ServicoEstoqueIndisponivelException : HttpRequestException
{
    public ServicoEstoqueIndisponivelException(string mensagem)
        : base(mensagem)
    {
    }

    public ServicoEstoqueIndisponivelException(string mensagem, Exception innerException)
        : base(mensagem, innerException)
    {
    }
}
