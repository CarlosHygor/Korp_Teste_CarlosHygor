namespace Faturamento.API.Models;

public class NotaFiscal
{
    public int Id { get; set; }

    public long Numeracao { get; set; }

    public StatusNotaFiscal Status { get; set; } = StatusNotaFiscal.Aberta;
    
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public List<ItemNotaFiscal> Itens { get; set; } = new();
}
