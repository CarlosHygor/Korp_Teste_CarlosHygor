using Faturamento.API.Data;
using Faturamento.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.API.Repositories;

public class ItemNotaFiscalRepository : IItemNotaFiscalRepository
{
    private readonly FaturamentoDbContext _context;

    public ItemNotaFiscalRepository(FaturamentoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ItemNotaFiscal>> GetByNotaFiscalIdAsync(int notaFiscalId)
    {
        return await _context.ItensNotaFiscal
                             .Where(i => i.NotaFiscalId == notaFiscalId)
                             .AsNoTracking()
                             .ToListAsync();
    }

    public async Task<ItemNotaFiscal?> GetByIdAsync(int id)
    {
        return await _context.ItensNotaFiscal
                             .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<ItemNotaFiscal> CreateAsync(ItemNotaFiscal item)
    {
        await _context.ItensNotaFiscal.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task DeleteAsync(ItemNotaFiscal item)
    {
        _context.ItensNotaFiscal.Remove(item);
        await _context.SaveChangesAsync();
    }
}
