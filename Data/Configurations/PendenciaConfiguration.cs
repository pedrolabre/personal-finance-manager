using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceManager.Data.Entities;

namespace PersonalFinanceManager.Data.Configurations;

public class PendenciaConfiguration : IEntityTypeConfiguration<Pendencia>
{
    public void Configure(EntityTypeBuilder<Pendencia> builder)
    {
        builder.ToTable("Pendencias");
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);
        
        builder.Property(p => p.ValorTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(p => p.DataCriacao)
            .IsRequired();
        
        builder.Property(p => p.Prioridade)
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(p => p.TipoDivida)
            .HasConversion<int>()
            .IsRequired();
        
        // Relacionamentos
        builder.HasOne(p => p.CartaoCredito)
            .WithMany(c => c.Pendencias)
            .HasForeignKey(p => p.CartaoCreditoId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(p => p.Parcelas)
            .WithOne(p => p.Pendencia)
            .HasForeignKey(p => p.PendenciaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(p => p.Acordos)
            .WithOne(a => a.Pendencia)
            .HasForeignKey(a => a.PendenciaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Índices
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.DataCriacao);
    }
}
