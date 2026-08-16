using Faturamento.API.Models;

namespace Faturamento.API.Services;

public interface INotaFiscalService
{
    Task<IEnumerable<NotaFiscal>> GetAllAsync();
    Task<NotaFiscal?> GetByIdAsync(int id);
    Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal);
    Task<NotaFiscal> ImprimirAsync(int id);
    Task UpdateAsync(int id, NotaFiscal notaFiscal);
    Task DeleteAsync(int id);
}
