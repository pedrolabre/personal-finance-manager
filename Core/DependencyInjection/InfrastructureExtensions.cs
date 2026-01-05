#nullable enable
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceManager.Core.Dialogs;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Navigation;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager.Core.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Core Services (Singleton para manter estado)
        services.AddSingleton<INavigationService>(provider =>
        {
            return new NavigationService(type =>
                (ViewModelBase)provider.GetRequiredService(type));
        });
        services.AddSingleton<IMessenger, Messenger>();
        services.AddSingleton<IDialogService, DialogService>();
        
        // AutoMapper
        services.AddAutoMapper(typeof(App));
        
        return services;
    }
}
