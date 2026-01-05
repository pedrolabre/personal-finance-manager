using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceManager.Data.Entities;

namespace PersonalFinanceManager.Data.Configurations;

public class AcordoConfiguration : IEntityTypeConfiguration<Acordo>
{
    public void Configure(EntityTypeBuilder<Acordo> builder)
    {
        builder.ToTable("Acordos");
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.DataAcordo)
            .IsRequired();
        
        builder.Property(a => a.NumeroParcelas)
            .IsRequired();
        
        builder.Property(a => a.ValorTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(a => a.Observacoes)
            .HasMaxLength(500);
        
        builder.Property(a => a.Ativo)
            .IsRequired()
            .HasDefaultValue(true);
        
        // Relacionamentos
        builder.HasMany(a => a.Parcelas)
            .WithOne(p => p.Acordo)
            .HasForeignKey(p => p.AcordoId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Índices
        builder.HasIndex(a => a.DataAcordo);
        builder.HasIndex(a => a.Ativo);
    }
}
