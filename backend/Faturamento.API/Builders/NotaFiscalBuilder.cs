using Faturamento.API.Models;

namespace Faturamento.API.Builders;

/// <summary>
/// Implementação do Design Pattern Builder para construção fluente de instâncias do Aggregate Root NotaFiscal.
/// </summary>
public class NotaFiscalBuilder
{
    private int _id;
    private long _numeracao;
    private StatusNotaFiscal _status = StatusNotaFiscal.Aberta;
    private DateTime _dataCriacao = DateTime.UtcNow;
    private readonly List<ItemNotaFiscal> _itens = new();

    public NotaFiscalBuilder ComId(int id)
    {
        _id = id;
        return this;
    }

    public NotaFiscalBuilder ComNumeracao(long numeracao)
    {
        _numeracao = numeracao;
        return this;
    }

    public NotaFiscalBuilder ComStatus(StatusNotaFiscal status)
    {
        _status = status;
        return this;
    }

    public NotaFiscalBuilder ComDataCriacao(DateTime dataCriacao)
    {
        _dataCriacao = dataCriacao;
        return this;
    }

    public NotaFiscalBuilder ComItem(string codigoProduto, string descricaoProduto, int quantidade)
    {
        var item = new ItemNotaFiscalBuilder()
            .ComCodigoProduto(codigoProduto)
            .ComDescricaoProduto(descricaoProduto)
            .ComQuantidade(quantidade)
            .Build();

        _itens.Add(item);
        return this;
    }

    public NotaFiscalBuilder ComItem(ItemNotaFiscal item)
    {
        _itens.Add(item);
        return this;
    }

    public NotaFiscal Build()
    {
        return new NotaFiscal
        {
            Id = _id,
            Numeracao = _numeracao,
            Status = _status,
            DataCriacao = _dataCriacao,
            Itens = _itens
        };
    }
}
