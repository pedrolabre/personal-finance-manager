using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceManager.Data.Entities;

namespace PersonalFinanceManager.Data.Configurations;

public class ParcelaConfiguration : IEntityTypeConfiguration<Parcela>
{
    public void Configure(EntityTypeBuilder<Parcela> builder)
    {
        builder.ToTable("Parcelas");
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.NumeroParcela)
            .IsRequired();
        
        builder.Property(p => p.Valor)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(p => p.DataVencimento)
            .IsRequired();
        
        builder.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(p => p.DataPagamento);
        
        // Índices
        builder.HasIndex(p => p.DataVencimento);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => new { p.PendenciaId, p.NumeroParcela });
    }
}
