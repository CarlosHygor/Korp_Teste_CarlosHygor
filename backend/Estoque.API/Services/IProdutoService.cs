using Estoque.API.DTOs;
using Estoque.API.Models;

namespace Estoque.API.Services;

public interface IProdutoService
{
    Task<IEnumerable<Produto>> GetAllAsync();
    Task<PagedResultDto<Produto>> GetPaginatedAsync(int pagina, int tamanhoPagina, string? ordenarPorSaldo = null, string? busca = null);
    Task<Produto?> GetByIdAsync(int id);
    Task<Produto?> GetByCodigoAsync(string codigo);
    Task<Produto> CreateAsync(Produto produto);
    Task UpdateAsync(int id, Produto produto);
    Task DeleteAsync(int id);
    Task AbaterEstoqueAsync(string codigo, int quantidade);
    Task AbaterEstoqueLoteAsync(IEnumerable<AbaterItemEstoqueDto> itens);
    Task EstornarEstoqueAsync(string codigo, int quantidade);
    Task EstornarEstoqueLoteAsync(IEnumerable<AbaterItemEstoqueDto> itens);
}
