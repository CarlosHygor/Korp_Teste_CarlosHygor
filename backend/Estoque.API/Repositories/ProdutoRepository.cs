using Estoque.API.Data;
using Estoque.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Estoque.API.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly EstoqueDbContext _context;

    public ProdutoRepository(EstoqueDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await _context.Produtos
                             .AsNoTracking()
                             .ToListAsync();
    }

    public async Task<(IEnumerable<Produto> Itens, int TotalCount)> GetPaginatedAsync(int pagina, int tamanhoPagina, string? ordenarPorSaldo = null)
    {
        var query = _context.Produtos.AsNoTracking();
        var totalCount = await query.CountAsync();

        if (ordenarPorSaldo?.ToLower() == "asc")
        {
            query = query.OrderBy(p => p.Saldo);
        }
        else if (ordenarPorSaldo?.ToLower() == "desc")
        {
            query = query.OrderByDescending(p => p.Saldo);
        }
        else
        {
            query = query.OrderBy(p => p.Id);
        }

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (itens, totalCount);
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        return await _context.Produtos
                             .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Produto?> GetByCodigoAsync(string codigo)
    {
        return await _context.Produtos
                             .FirstOrDefaultAsync(p => p.Codigo == codigo);
    }

    public async Task<Produto> AddAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();
        return produto;
    }

    public async Task UpdateAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }
    }
}
