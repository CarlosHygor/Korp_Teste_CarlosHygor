namespace Estoque.API.Exceptions;

public class EstoqueInsuficienteException : Exception
{
    public string CodigoProduto { get; }
    public int SaldoAtual { get; }
    public int QuantidadeSolicitada { get; }

    public EstoqueInsuficienteException(string codigoProduto, int saldoAtual, int quantidadeSolicitada)
        : base($"Estoque insuficiente para o produto '{codigoProduto}'. Saldo disponível: {saldoAtual}, quantidade solicitada: {quantidadeSolicitada}.")
    {
        CodigoProduto = codigoProduto;
        SaldoAtual = saldoAtual;
        QuantidadeSolicitada = quantidadeSolicitada;
    }
}
