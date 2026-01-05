using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceManager.TempModels;

public partial class FinanceContext : DbContext
{
    public FinanceContext()
    {
    }

    public FinanceContext(DbContextOptions<FinanceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Acordo> Acordos { get; set; }

    public virtual DbSet<CartoesCredito> CartoesCreditos { get; set; }

    public virtual DbSet<EfmigrationsLock> EfmigrationsLocks { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Parcela> Parcelas { get; set; }

    public virtual DbSet<Pendencia> Pendencias { get; set; }

    public virtual DbSet<Recebimento> Recebimentos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=C:\\Users\\pedro\\AppData\\Local\\PersonalFinanceManager\\finance.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Acordo>(entity =>
        {
            entity.HasIndex(e => e.Ativo, "IX_Acordos_Ativo");

            entity.HasIndex(e => e.DataAcordo, "IX_Acordos_DataAcordo");

            entity.HasIndex(e => e.PendenciaId, "IX_Acordos_PendenciaId");

            entity.Property(e => e.Ativo).HasDefaultValue(1);
            entity.Property(e => e.ValorTotal).HasColumnType("decimal(18,2)");

            entity.HasOne(d => d.Pendencia).WithMany(p => p.Acordos).HasForeignKey(d => d.PendenciaId);
        });

        modelBuilder.Entity<CartoesCredito>(entity =>
        {
            entity.ToTable("CartoesCredito");

            entity.HasIndex(e => e.Ativo, "IX_CartoesCredito_Ativo");

            entity.HasIndex(e => e.Nome, "IX_CartoesCredito_Nome");

            entity.Property(e => e.Ativo).HasDefaultValue(1);
            entity.Property(e => e.Limite).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Nome).IsRequired();
        });

        modelBuilder.Entity<EfmigrationsLock>(entity =>
        {
            entity.ToTable("__EFMigrationsLock");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Timestamp).IsRequired();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(e => e.DataAgendamento, "IX_Notifications_DataAgendamento");

            entity.HasIndex(e => e.Enviada, "IX_Notifications_Enviada");

            entity.HasIndex(e => e.NotificationId, "IX_Notifications_NotificationId").IsUnique();

            entity.Property(e => e.DataAgendamento).IsRequired();
            entity.Property(e => e.Mensagem).IsRequired();
            entity.Property(e => e.NotificationId).IsRequired();
            entity.Property(e => e.Titulo).IsRequired();
        });

        modelBuilder.Entity<Parcela>(entity =>
        {
            entity.HasIndex(e => e.AcordoId, "IX_Parcelas_AcordoId");

            entity.HasIndex(e => e.DataVencimento, "IX_Parcelas_DataVencimento");

            entity.HasIndex(e => new { e.PendenciaId, e.NumeroParcela }, "IX_Parcelas_PendenciaId_NumeroParcela");

            entity.HasIndex(e => e.Status, "IX_Parcelas_Status");

            entity.Property(e => e.Valor).HasColumnType("decimal(18,2)");

            entity.HasOne(d => d.Acordo).WithMany(p => p.Parcelas)
                .HasForeignKey(d => d.AcordoId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Pendencia).WithMany(p => p.Parcelas).HasForeignKey(d => d.PendenciaId);
        });

        modelBuilder.Entity<Pendencia>(entity =>
        {
            entity.HasIndex(e => e.CartaoCreditoId, "IX_Pendencias_CartaoCreditoId");

            entity.HasIndex(e => e.DataCriacao, "IX_Pendencias_DataCriacao");

            entity.HasIndex(e => e.Status, "IX_Pendencias_Status");

            entity.Property(e => e.Nome).IsRequired();
            entity.Property(e => e.ValorTotal).HasColumnType("decimal(18,2)");

            entity.HasOne(d => d.CartaoCredito).WithMany(p => p.Pendencia)
                .HasForeignKey(d => d.CartaoCreditoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Recebimento>(entity =>
        {
            entity.HasIndex(e => e.DataPrevista, "IX_Recebimentos_DataPrevista");

            entity.HasIndex(e => e.RecebimentoCompleto, "IX_Recebimentos_RecebimentoCompleto");

            entity.Property(e => e.Descricao).IsRequired();
            entity.Property(e => e.ValorEsperado).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ValorRecebido)
                .HasDefaultValueSql("'0.0'")
                .HasColumnType("decimal(18,2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
