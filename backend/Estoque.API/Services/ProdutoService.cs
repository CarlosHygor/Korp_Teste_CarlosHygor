using Estoque.API.Exceptions;
using Estoque.API.Models;
using Estoque.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Estoque.API.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await _produtoRepository.GetAllAsync();
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

        try {
            return await _produtoRepository.AddAsync(produto);
        } catch (DbUpdateException) {
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
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
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
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
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
            throw new KeyNotFoundException($"Produto com código '{codigo}' não encontrado no estoque.");
        }

        if (produto.Saldo < quantidade)
        {
            throw new EstoqueInsuficienteException(codigo, produto.Saldo, quantidade);
        }

        produto.Saldo -= quantidade;
        await _produtoRepository.UpdateAsync(produto);
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