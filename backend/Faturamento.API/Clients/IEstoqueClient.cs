namespace Faturamento.API.Clients;

public interface IEstoqueClient
{
    Task AbaterEstoqueAsync(string codigoProduto, int quantidade);
}
