using Faturamento.API.DTOs;
using Faturamento.API.Models;

namespace Faturamento.API.Services;

public interface INotaFiscalService
{
    Task<IEnumerable<NotaFiscal>> GetAllAsync();
    Task<PagedResultDto<NotaFiscal>> GetPaginatedAsync(int pagina, int tamanhoPagina);
    Task<NotaFiscal?> GetByIdAsync(int id);
    Task<NotaFiscal?> GetByNumeracaoAsync(long numeracao);
    Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal);
    Task<NotaFiscal> ImprimirAsync(int id);
    Task UpdateAsync(int id, NotaFiscal notaFiscal);
    Task DeleteAsync(int id);
}
