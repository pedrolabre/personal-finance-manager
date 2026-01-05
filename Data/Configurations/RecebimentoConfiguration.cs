using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceManager.Data.Entities;

namespace PersonalFinanceManager.Data.Configurations;

public class RecebimentoConfiguration : IEntityTypeConfiguration<Recebimento>
{
    public void Configure(EntityTypeBuilder<Recebimento> builder)
    {
        builder.ToTable("Recebimentos");
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Descricao)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(r => r.Categoria)
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(r => r.DataPrevista)
            .IsRequired();
        
        builder.Property(r => r.DataRecebimento);
        
        builder.Property(r => r.ValorEsperado)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(r => r.ValorRecebido)
            .HasColumnType("decimal(18,2)")
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(r => r.RecebimentoCompleto)
            .IsRequired()
            .HasDefaultValue(false);
        
        // Ignorar propriedades calculadas
        builder.Ignore(r => r.ValorPendente);
        builder.Ignore(r => r.Atrasado);
        
        // Índices
        builder.HasIndex(r => r.DataPrevista);
        builder.HasIndex(r => r.RecebimentoCompleto);
    }
}
