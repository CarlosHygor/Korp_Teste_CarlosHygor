using Estoque.API.Data;
using Estoque.API.DTOs;
using Estoque.API.Exceptions;
using Estoque.API.Models;
using Estoque.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Estoque.API.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly EstoqueDbContext _context;

    public ProdutoService(IProdutoRepository produtoRepository, EstoqueDbContext context)
    {
        _produtoRepository = produtoRepository;
        _context = context;
    }

    public async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await _produtoRepository.GetAllAsync();
    }

    public async Task<PagedResultDto<Produto>> GetPaginatedAsync(int pagina, int tamanhoPagina, string? ordenarPorSaldo = null)
    {
        var paginaValida = pagina <= 0 ? 1 : pagina;
        var tamanhoValido = tamanhoPagina <= 0 ? 10 : (tamanhoPagina > 100 ? 100 : tamanhoPagina);

        var (itens, totalRegistros) = await _produtoRepository.GetPaginatedAsync(paginaValida, tamanhoValido, ordenarPorSaldo);

        var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanhoValido);
        var temPaginaAnterior = paginaValida > 1;
        var temProximaPagina = paginaValida < totalPaginas;

        return new PagedResultDto<Produto>(
            itens,
            paginaValida,
            tamanhoValido,
            totalRegistros,
            totalPaginas,
            temPaginaAnterior,
            temProximaPagina
        );
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        return await _produtoRepository.GetByIdAsync(id);
    }

    public async Task<Produto?> GetByCodigoAsync(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            throw new ArgumentException("O código do produto não pode ser nulo ou vazio.", nameof(codigo));
        }

        return await _produtoRepository.GetByCodigoAsync(codigo);
    }

    public async Task<Produto> CreateAsync(Produto produto)
    {
        ValidarDadosProduto(produto);

        try
        {
            return await _produtoRepository.AddAsync(produto);
        }
        catch (DbUpdateException)
        {
            // Captura a violação de índice único do PostgreSQL
            throw new CodigoProdutoDuplicadoException(produto.Codigo);
        }
    }

    public async Task UpdateAsync(int id, Produto produtoAtualizado)
    {
        ValidarDadosProduto(produtoAtualizado);

        var produtoExistente = await _produtoRepository.GetByIdAsync(id);
        if (produtoExistente == null)
        {
            throw new ProdutoNaoEncontradoException(id);
        }

        produtoExistente.Codigo = produtoAtualizado.Codigo;
        produtoExistente.Descricao = produtoAtualizado.Descricao;
        produtoExistente.Saldo = produtoAtualizado.Saldo;

        try
        {
            await _produtoRepository.UpdateAsync(produtoExistente);
        }
        catch (DbUpdateException)
        {
            throw new CodigoProdutoDuplicadoException(produtoAtualizado.Codigo);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var produtoExistente = await _produtoRepository.GetByIdAsync(id);
        if (produtoExistente == null)
        {
            throw new ProdutoNaoEncontradoException(id);
        }

        await _produtoRepository.DeleteAsync(id);
    }

    public async Task AbaterEstoqueAsync(string codigo, int quantidade)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            throw new ArgumentException("O código do produto deve ser informado.", nameof(codigo));
        }

        if (quantidade <= 0)
        {
            throw new ArgumentException("A quantidade a abater deve ser maior que zero.", nameof(quantidade));
        }

        var produto = await _produtoRepository.GetByCodigoAsync(codigo);
        if (produto == null)
        {
            throw new ProdutoNaoEncontradoException(codigo, true);
        }

        if (produto.Saldo < quantidade)
        {
            throw new EstoqueInsuficienteException(codigo, produto.Saldo, quantidade);
        }

        produto.Saldo -= quantidade;
        await _produtoRepository.UpdateAsync(produto);
    }

    public async Task AbaterEstoqueLoteAsync(IEnumerable<AbaterItemEstoqueDto> itens)
    {
        var listaItens = itens?.ToList() ?? new List<AbaterItemEstoqueDto>();
        if (!listaItens.Any())
        {
            throw new ArgumentException("A lista de itens para abate de estoque não pode estar vazia.");
        }

        // Reutiliza a lógica de recomposição individual e garante a atomicidade das operações
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in listaItens)
            {
                await AbaterEstoqueAsync(item.CodigoProduto, item.Quantidade);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task EstornarEstoqueAsync(string codigo, int quantidade)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            throw new ArgumentException("O código do produto deve ser informado.", nameof(codigo));
        }

        if (quantidade <= 0)
        {
            throw new ArgumentException("A quantidade a estornar deve ser maior que zero.", nameof(quantidade));
        }

        var produto = await _produtoRepository.GetByCodigoAsync(codigo);
        if (produto == null)
        {
            throw new ProdutoNaoEncontradoException(codigo, true);
        }

        produto.Saldo += quantidade;
        await _produtoRepository.UpdateAsync(produto);
    }

    public async Task EstornarEstoqueLoteAsync(IEnumerable<AbaterItemEstoqueDto> itens)
    {
        var listaItens = itens?.ToList() ?? new List<AbaterItemEstoqueDto>();
        if (!listaItens.Any())
        {
            return;
        }

        // Reutiliza a lógica de recomposição individual e garante a atomicidade das operações
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in listaItens)
            {
                await EstornarEstoqueAsync(item.CodigoProduto, item.Quantidade);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static void ValidarDadosProduto(Produto produto)
    {
        if (string.IsNullOrWhiteSpace(produto.Codigo))
        {
            throw new ArgumentException("O código do produto é um campo obrigatório.", nameof(produto.Codigo));
        }

        if (string.IsNullOrWhiteSpace(produto.Descricao))
        {
            throw new ArgumentException("A descrição do produto é um campo obrigatório.", nameof(produto.Descricao));
        }

        if (produto.Saldo < 0)
        {
            throw new ArgumentException("O saldo do produto não pode ser negativo.", nameof(produto.Saldo));
        }
    }
}