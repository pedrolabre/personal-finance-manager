#nullable enable
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceManager.Repositories.Implementations;
using PersonalFinanceManager.Repositories.Interfaces;

namespace PersonalFinanceManager.Core.DependencyInjection;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPendenciaRepository, PendenciaRepository>();
        services.AddScoped<ICartaoCreditoRepository, CartaoCreditoRepository>();
        services.AddScoped<IParcelaRepository, ParcelaRepository>();
        services.AddScoped<IAcordoRepository, AcordoRepository>();
        services.AddScoped<IRecebimentoRepository, RecebimentoRepository>();
        
        return services;
    }
}
