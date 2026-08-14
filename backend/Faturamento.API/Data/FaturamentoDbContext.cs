using Microsoft.EntityFrameworkCore;

namespace Faturamento.API.Data;

public class FaturamentoDbContext : DbContext
{
    public FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : base(options)
    {
    }

    // Os DbSets das tabelas de Faturamento (ex: NotaFiscal, ItemNotaFiscal) serão adicionados nos próximos passos.
}
