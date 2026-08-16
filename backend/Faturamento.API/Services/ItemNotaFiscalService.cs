using Faturamento.API.Clients;
using Faturamento.API.Models;

namespace Faturamento.API.Services;

public class ItemNotaFiscalService : IItemNotaFiscalService
{
    private readonly IEstoqueClient _estoqueClient;

    public ItemNotaFiscalService(IEstoqueClient estoqueClient)
    {
        _estoqueClient = estoqueClient;
    }

    public async Task AbaterEstoqueAsync(ItemNotaFiscal item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "O item da nota fiscal não pode ser nulo.");
        }

        if (string.IsNullOrWhiteSpace(item.CodigoProduto))
        {
            throw new ArgumentException("O código do produto é obrigatório para abater o estoque.");
        }

        if (item.Quantidade <= 0)
        {
            throw new ArgumentException($"A quantidade a abater do produto '{item.CodigoProduto}' deve ser maior que zero.");
        }

        await _estoqueClient.AbaterEstoqueAsync(item.CodigoProduto, item.Quantidade);
    }
}
