using Faturamento.API.Models;

namespace Faturamento.API.Services;

public interface IItemNotaFiscalService
{
    Task AbaterEstoqueAsync(ItemNotaFiscal item);
}
