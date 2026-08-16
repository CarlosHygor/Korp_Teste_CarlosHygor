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
