using Faturamento.API.Models;

namespace Faturamento.API.Builders;

/// <summary>
/// Implementação do Design Pattern Builder para construção fluente de instâncias de ItemNotaFiscal.
/// </summary>
public class ItemNotaFiscalBuilder
{
    private int _id;
    private string _codigoProduto = string.Empty;
    private string _descricaoProduto = string.Empty;
    private int _quantidade;
    private int _notaFiscalId;

    public ItemNotaFiscalBuilder ComId(int id)
    {
        _id = id;
        return this;
    }

    public ItemNotaFiscalBuilder ComCodigoProduto(string codigoProduto)
    {
        _codigoProduto = codigoProduto;
        return this;
    }

    public ItemNotaFiscalBuilder ComDescricaoProduto(string descricaoProduto)
    {
        _descricaoProduto = descricaoProduto;
        return this;
    }

    public ItemNotaFiscalBuilder ComQuantidade(int quantidade)
    {
        _quantidade = quantidade;
        return this;
    }

    public ItemNotaFiscalBuilder ComNotaFiscalId(int notaFiscalId)
    {
        _notaFiscalId = notaFiscalId;
        return this;
    }

    public ItemNotaFiscal Build()
    {
        return new ItemNotaFiscal
        {
            Id = _id,
            CodigoProduto = _codigoProduto,
            DescricaoProduto = _descricaoProduto,
            Quantidade = _quantidade,
            NotaFiscalId = _notaFiscalId
        };
    }
}
