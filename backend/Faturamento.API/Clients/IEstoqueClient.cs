using Faturamento.API.Clients.DTOs;

namespace Faturamento.API.Clients;

public interface IEstoqueClient
{
    Task AbaterEstoqueAsync(string codigoProduto, int quantidade);
    Task AbaterEstoqueLoteAsync(IEnumerable<ItemAbateEstoqueDto> itens);
    Task EstornarEstoqueLoteAsync(IEnumerable<ItemAbateEstoqueDto> itens);
}
