using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceManager.Data.Entities;

namespace PersonalFinanceManager.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);
        
        builder.Property(n => n.NotificationId)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(n => n.ReferenceId)
            .IsRequired();
        
        builder.Property(n => n.Titulo)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(n => n.Mensagem)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(n => n.DataAgendamento)
            .IsRequired();
        
        builder.Property(n => n.Tipo)
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(n => n.Enviada)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(n => n.Cancelada)
            .IsRequired()
            .HasDefaultValue(false);
        
        // Índices
        builder.HasIndex(n => n.NotificationId).IsUnique();
        builder.HasIndex(n => n.DataAgendamento);
        builder.HasIndex(n => n.Enviada);
    }
}