using Faturamento.API.Data;
using Faturamento.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.API.Repositories;

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly FaturamentoDbContext _context;

    public NotaFiscalRepository(FaturamentoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NotaFiscal>> GetAllAsync()
    {
        return await _context.NotasFiscais
                             .Include(n => n.Itens)
                             .AsNoTracking()
                             .OrderByDescending(n => n.DataCriacao)
                             .ToListAsync();
    }

    public async Task<(IEnumerable<NotaFiscal> Itens, int TotalCount)> GetPaginatedAsync(int pagina, int tamanhoPagina, StatusNotaFiscal? status = null, string? ordenacao = null)
    {
        var query = _context.NotasFiscais.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(n => n.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        query = ordenacao?.ToLower() switch
        {
            "data_asc" => query.OrderBy(n => n.DataCriacao),
            "itens_asc" => query.OrderBy(n => n.Itens.Count),
            "itens_desc" => query.OrderByDescending(n => n.Itens.Count),
            _ => query.OrderByDescending(n => n.DataCriacao)
        };

        var itens = await query
            .Include(n => n.Itens)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (itens, totalCount);
    }

    public async Task<NotaFiscal?> GetByIdAsync(int id)
    {
        return await _context.NotasFiscais
                             .Include(n => n.Itens)
                             .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<NotaFiscal?> GetByNumeracaoAsync(long numeracao)
    {
        return await _context.NotasFiscais
                             .Include(n => n.Itens)
                             .FirstOrDefaultAsync(n => n.Numeracao == numeracao);
    }

    public async Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal)
    {
        await _context.NotasFiscais.AddAsync(notaFiscal);
        await _context.SaveChangesAsync();
        return notaFiscal;
    }

    public async Task UpdateAsync(NotaFiscal notaFiscal)
    {
        _context.NotasFiscais.Update(notaFiscal);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(NotaFiscal notaFiscal)
    {
        _context.NotasFiscais.Remove(notaFiscal);
        await _context.SaveChangesAsync();
    }
}
