using Estoque.API.Models;

namespace Estoque.API.Builders;

/// <summary>
/// Implementação do Design Pattern Builder para criação fluente de instâncias de Produto.
/// </summary>
public class ProdutoBuilder
{
    private int _id;
    private string _codigo = string.Empty;
    private string _descricao = string.Empty;
    private int _saldo;

    public ProdutoBuilder ComId(int id)
    {
        _id = id;
        return this;
    }

    public ProdutoBuilder ComCodigo(string codigo)
    {
        _codigo = codigo;
        return this;
    }

    public ProdutoBuilder ComDescricao(string descricao)
    {
        _descricao = descricao;
        return this;
    }

    public ProdutoBuilder ComSaldo(int saldo)
    {
        _saldo = saldo;
        return this;
    }

    public Produto Build()
    {
        return new Produto
        {
            Id = _id,
            Codigo = _codigo,
            Descricao = _descricao,
            Saldo = _saldo
        };
    }
}
