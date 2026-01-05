#nullable enable
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceManager.ViewModels;
using PersonalFinanceManager.ViewModels.Acordos;
using PersonalFinanceManager.ViewModels.Cartoes;
using PersonalFinanceManager.ViewModels.Configuracoes;
using PersonalFinanceManager.ViewModels.Importacao;
using PersonalFinanceManager.ViewModels.Pendencias;
using PersonalFinanceManager.ViewModels.Recebimentos;
using PersonalFinanceManager.ViewModels.Relatorios;

namespace PersonalFinanceManager.Core.DependencyInjection;

public static class ViewModelExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        // Main ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        
        // Pendências ViewModels
        services.AddTransient<PendenciasListViewModel>();
        services.AddTransient<PendenciaFormViewModel>();
        services.AddTransient<PendenciaDetalhesViewModel>();
        
        // Cartões ViewModels
        services.AddTransient<CartoesListViewModel>();
        services.AddTransient<CartaoFormViewModel>();
        
        // Acordos ViewModels
        services.AddTransient<AcordosListViewModel>();
        services.AddTransient<AcordoFormViewModel>();
        services.AddTransient<AcordoDetalhesViewModel>();
        
        // Recebimentos ViewModels
        services.AddTransient<RecebimentosListViewModel>();
        services.AddTransient<RecebimentoFormViewModel>();
        
        // Other ViewModels
        services.AddTransient<ImportacaoViewModel>();
        services.AddTransient<RelatoriosViewModel>();
        services.AddTransient<ConfiguracoesViewModel>();
        
        return services;
    }
}
