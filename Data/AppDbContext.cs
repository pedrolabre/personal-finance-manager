using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data.Configurations;
using PersonalFinanceManager.Data.Entities;

namespace PersonalFinanceManager.Data;

public class AppDbContext : DbContext
{
    public DbSet<Pendencia> Pendencias { get; set; } = null!;
    public DbSet<Parcela> Parcelas { get; set; } = null!;
    public DbSet<CartaoCredito> CartoesCredito { get; set; } = null!;
    public DbSet<Acordo> Acordos { get; set; } = null!;
    public DbSet<Recebimento> Recebimentos { get; set; } = null!;
    public DbSet<NotificationEntity> Notifications { get; set; } = null!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aplicar todas as configurações
        modelBuilder.ApplyConfiguration(new PendenciaConfiguration());
        modelBuilder.ApplyConfiguration(new ParcelaConfiguration());
        modelBuilder.ApplyConfiguration(new CartaoCreditoConfiguration());
        modelBuilder.ApplyConfiguration(new AcordoConfiguration());
        modelBuilder.ApplyConfiguration(new RecebimentoConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PersonalFinanceManager",
                "finance.db");
            
            var directory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}