using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceManager.Data.Entities;

namespace PersonalFinanceManager.Data.Configurations;

public class CartaoCreditoConfiguration : IEntityTypeConfiguration<CartaoCredito>
{
    public void Configure(EntityTypeBuilder<CartaoCredito> builder)
    {
        builder.ToTable("CartoesCredito");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(c => c.Banco)
            .HasMaxLength(100);
        
        builder.Property(c => c.DiaVencimento)
            .IsRequired();
        
        builder.Property(c => c.DiaFechamento)
            .IsRequired();
        
        builder.Property(c => c.Limite)
            .HasColumnType("decimal(18,2)");
        
        builder.Property(c => c.Ativo)
            .IsRequired()
            .HasDefaultValue(true);
        
        // Índices
        builder.HasIndex(c => c.Nome);
        builder.HasIndex(c => c.Ativo);
    }
}
