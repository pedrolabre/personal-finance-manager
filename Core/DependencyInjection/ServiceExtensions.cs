#nullable enable
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceManager.Services.Implementations;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.Services.Import;
using PersonalFinanceManager.Services.Import.Parsers;
using PersonalFinanceManager.Services.Import.Parsers.Strategies;

namespace PersonalFinanceManager.Core.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // Business Services
        services.AddScoped<IPendenciaService, PendenciaService>();
        services.AddScoped<ICartaoCreditoService, CartaoCreditoService>();
        services.AddScoped<IAcordoService, AcordoService>();
        services.AddScoped<IRecebimentoService, RecebimentoService>();
        
        // Import Services & Strategies
        services.AddScoped<IImportService, ImportService>();
        services.AddScoped<ITextParser, CsvParser>();
        
        // CSV Parsing Strategies (Strategy Pattern)
        services.AddScoped<ICsvParsingStrategy, GenericCsvStrategy>();
        services.AddScoped<ICsvParsingStrategy, NubankCsvStrategy>();
        services.AddScoped<ICsvParsingStrategy, InterCsvStrategy>();
        services.AddScoped<CsvStrategyFactory>();
        
        // Report Services
        services.AddScoped<Services.Reports.IReportService, Services.Reports.ReportService>();
        
        return services;
    }
}
