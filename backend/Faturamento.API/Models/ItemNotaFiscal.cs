using System.Text.Json.Serialization;

namespace Faturamento.API.Models;

public class ItemNotaFiscal
{
    public int Id { get; set; }
    
    public int NotaFiscalId { get; set; }

    [JsonIgnore]
    public NotaFiscal? NotaFiscal { get; set; }

    // Chave candidata do produto no Estoque 
    public string CodigoProduto { get; set; } = string.Empty;

    // Snapshot da descrição do produto
    public string DescricaoProduto { get; set; } = string.Empty;

    public int Quantidade { get; set; }
}
