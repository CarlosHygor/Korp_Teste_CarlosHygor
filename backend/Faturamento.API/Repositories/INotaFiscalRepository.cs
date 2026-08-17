using Faturamento.API.Models;

namespace Faturamento.API.Repositories;

public interface INotaFiscalRepository
{
    Task<IEnumerable<NotaFiscal>> GetAllAsync();
    Task<(IEnumerable<NotaFiscal> Itens, int TotalCount)> GetPaginatedAsync(int pagina, int tamanhoPagina, StatusNotaFiscal? status = null, string? ordenacao = null);
    Task<NotaFiscal?> GetByIdAsync(int id);
    Task<NotaFiscal?> GetByNumeracaoAsync(long numeracao);
    Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal);
    Task UpdateAsync(NotaFiscal notaFiscal);
    Task DeleteAsync(NotaFiscal notaFiscal);
}
