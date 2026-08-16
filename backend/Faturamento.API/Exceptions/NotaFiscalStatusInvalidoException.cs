namespace Faturamento.API.Exceptions;

public class NotaFiscalStatusInvalidoException : InvalidOperationException
{
    public NotaFiscalStatusInvalidoException(string mensagem)
        : base(mensagem)
    {
    }

    public NotaFiscalStatusInvalidoException(string mensagem, Exception innerException)
        : base(mensagem, innerException)
    {
    }
}
