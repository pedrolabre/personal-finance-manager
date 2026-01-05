
using System.Windows;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceManager.Core.DependencyInjection;
using PersonalFinanceManager.Core.Navigation;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Services.Implementations;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.ViewModels;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager;

public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    public App()
    {
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        // Mostrar erro para diagnóstico
        MessageBox.Show($"Erro não tratado: {e.Exception.Message}\n\n{e.Exception.StackTrace}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // Mostrar erro para diagnóstico
        if (e.ExceptionObject is Exception ex)
        {
            MessageBox.Show($"Erro fatal: {ex.Message}\n\n{ex.StackTrace}", "Erro Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Configurar licença comunitária do QuestPDF
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        
        // Garantir que o diretório do banco existe
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PersonalFinanceManager",
            "finance.db");
        var dbDir = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
        {
            Directory.CreateDirectory(dbDir);
        }

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Aplicar migrations sempre que iniciar, mesmo se o arquivo já existir
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao aplicar migrations do banco de dados: {ex.Message}", "Erro de Banco de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        // Criação manual da MainWindow com DataContext injetado
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        var mainWindow = new MainWindow
        {
            DataContext = mainViewModel
        };
        mainWindow.Show();
        
        base.OnStartup(e);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PersonalFinanceManager",
                "finance.db");

            options.UseSqlite($"Data Source={dbPath}");
        });

        // Infrastructure (Navigation, Messaging, Dialogs, AutoMapper)
        services.AddInfrastructure();
        
        // Repositories
        services.AddRepositories();
        
        // Business Services
        services.AddBusinessServices();
        
        // Dashboard Service
        services.AddScoped<IDashboardService, DashboardService>();
        
        // ViewModels
        services.AddViewModels();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
