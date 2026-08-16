namespace Faturamento.API.Exceptions;

public class NotaFiscalNaoEncontradaException : KeyNotFoundException
{
    public NotaFiscalNaoEncontradaException(string mensagem)
        : base(mensagem)
    {
    }

    public NotaFiscalNaoEncontradaException(int id)
        : base($"Nota Fiscal com ID {id} não foi encontrada.")
    {
    }

    public NotaFiscalNaoEncontradaException(long numeracao)
        : base($"Nota Fiscal com numeração '{numeracao}' não foi encontrada.")
    {
    }
}
