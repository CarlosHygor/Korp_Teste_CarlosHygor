using Estoque.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Estoque.API.Data;

public class EstoqueDbContext : DbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options)
    {
    }

    // DbSet representa a tabela 'Produtos' no banco de dados (equivalente ao JpaRepository/EntityManager no Spring)
    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeamento e restrições da tabela 'produtos' no PostgreSQL
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("produtos", t => t.HasCheckConstraint("CK_produtos_saldo", "\"Saldo\" >= 0"));

            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Codigo)
                  .IsRequired()
                  .HasMaxLength(50);
            
            // Garantir que o Código do Produto seja Único no banco de dados
            entity.HasIndex(p => p.Codigo)
                  .IsUnique();

            entity.Property(p => p.Descricao)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(p => p.Saldo)
                  .IsRequired();
        });
    }
}
