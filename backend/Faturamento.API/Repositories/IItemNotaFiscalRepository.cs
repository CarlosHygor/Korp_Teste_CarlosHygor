using Faturamento.API.Models;

namespace Faturamento.API.Repositories;

public interface IItemNotaFiscalRepository
{
    Task<IEnumerable<ItemNotaFiscal>> GetByNotaFiscalIdAsync(int notaFiscalId);
    Task<ItemNotaFiscal?> GetByIdAsync(int id);
    Task<ItemNotaFiscal> CreateAsync(ItemNotaFiscal item);
    Task DeleteAsync(ItemNotaFiscal item);
}
