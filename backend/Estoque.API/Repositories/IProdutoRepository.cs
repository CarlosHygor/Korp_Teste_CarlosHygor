using Estoque.API.Models;

namespace Estoque.API.Repositories;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> GetAllAsync();
    Task<(IEnumerable<Produto> Itens, int TotalCount)> GetPaginatedAsync(int pagina, int tamanhoPagina);
    Task<Produto?> GetByIdAsync(int id);
    Task<Produto?> GetByCodigoAsync(string codigo);
    Task<Produto> AddAsync(Produto produto);
    Task UpdateAsync(Produto produto);
    Task DeleteAsync(int id);
}
