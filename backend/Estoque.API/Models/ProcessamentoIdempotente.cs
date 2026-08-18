namespace Estoque.API.Models;

public class ProcessamentoIdempotente
{
    public long Id { get; set; }
    public string Chave { get; set; } = string.Empty;
    public DateTime DataProcessamentoUtc { get; set; } = DateTime.UtcNow;
}
