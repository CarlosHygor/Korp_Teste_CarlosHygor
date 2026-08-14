using Microsoft.EntityFrameworkCore;

namespace Estoque.API.Data;

public class EstoqueDbContext : DbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options)
    {
    }

    // Os DbSets das tabelas de Estoque (ex: Produto) serão adicionados nos próximos passos.
}
