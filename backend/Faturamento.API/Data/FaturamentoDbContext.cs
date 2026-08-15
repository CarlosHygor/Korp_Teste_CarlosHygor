using Faturamento.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.API.Data;

public class FaturamentoDbContext : DbContext
{
    public FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : base(options)
    {
    }

    public DbSet<NotaFiscal> NotasFiscais => Set<NotaFiscal>();
    public DbSet<ItemNotaFiscal> ItensNotaFiscal => Set<ItemNotaFiscal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeamento da tabela 'notas_fiscais'
        modelBuilder.Entity<NotaFiscal>(entity =>
        {
            entity.ToTable("notas_fiscais");

            entity.HasKey(n => n.Id);

            // Numeração sequencial gerada automaticamente pelo banco (ValueGeneratedOnAdd / Identity / Sequence)
            entity.Property(n => n.Numeracao)
                  .ValueGeneratedOnAdd();

            // Garantir que a numeração da nota fiscal seja única
            entity.HasIndex(n => n.Numeracao)
                  .IsUnique();

            // Grava o enum de Status como texto ("Aberta", "Fechada") no Postgres
            entity.Property(n => n.Status)
                  .IsRequired()
                  .HasConversion<string>();

            entity.Property(n => n.DataCriacao)
                  .IsRequired();

            // Configuração do relacionamento 1:N entre NotaFiscal e ItemNotaFiscal
            entity.HasMany(n => n.Itens)
                  .WithOne(i => i.NotaFiscal)
                  .HasForeignKey(i => i.NotaFiscalId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Mapeamento da tabela 'itens_nota_fiscal'
        modelBuilder.Entity<ItemNotaFiscal>(entity =>
        {
            entity.ToTable("itens_nota_fiscal");

            entity.HasKey(i => i.Id);

            entity.Property(i => i.CodigoProduto)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(i => i.DescricaoProduto)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(i => i.Quantidade)
                  .IsRequired();
        });
    }
}
