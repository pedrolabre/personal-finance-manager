using System.Threading.Tasks;
using PersonalFinanceManager.Services.Reports.Models;

namespace PersonalFinanceManager.Services.Reports
{
    public interface IReportService
    {
        Task<string> GerarRelatorioPendenciasAsync(ReportOptions options);
        Task<string> GerarRelatorioCartoesAsync(ReportOptions options);
        Task<string> GerarRelatorioDashboardAsync();
    }
}
