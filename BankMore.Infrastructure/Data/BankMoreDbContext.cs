using BankMore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Infrastructure.Data;

public class BankMoreDbContext : DbContext
{
    public BankMoreDbContext(DbContextOptions<BankMoreDbContext> options) : base(options)
    {
    }

    public DbSet<ContaCorrente> ContasCorrentes { get; set; }
    public DbSet<Movimento> Movimentos { get; set; }
    public DbSet<Transferencia> Transferencias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContaCorrente>(entity =>
        {
            entity.ToTable("CONTACORRENTE");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cpf).IsRequired().HasMaxLength(11);
            entity.Property(e => e.SenhaHash).IsRequired();
            entity.Property(e => e.NumeroConta).IsRequired().HasMaxLength(20);
            entity.Property(e => e.NomeTitular).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Ativo).IsRequired();
            entity.HasIndex(e => e.Cpf);
            entity.HasIndex(e => e.NumeroConta).IsUnique();
        });

        modelBuilder.Entity<Movimento>(entity =>
        {
            entity.ToTable("MOVIMENTO");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdentificacaoRequisicao).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Valor).HasPrecision(18, 2);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(1);
            entity.HasOne(e => e.ContaCorrente)
                .WithMany(c => c.Movimentos)
                .HasForeignKey(e => e.ContaCorrenteId);
            entity.HasIndex(e => new { e.IdentificacaoRequisicao, e.ContaCorrenteId });
        });

        modelBuilder.Entity<Transferencia>(entity =>
        {
            entity.ToTable("TRANSFERENCIA");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdentificacaoRequisicao).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Valor).HasPrecision(18, 2);
            entity.HasOne(e => e.ContaOrigem)
                .WithMany(c => c.TransferenciasOrigem)
                .HasForeignKey(e => e.ContaOrigemId);
            entity.HasOne(e => e.ContaDestino)
                .WithMany(c => c.TransferenciasDestino)
                .HasForeignKey(e => e.ContaDestinoId);
            entity.HasIndex(e => e.IdentificacaoRequisicao);
        });
    }
}

