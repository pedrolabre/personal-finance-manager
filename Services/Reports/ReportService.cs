using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceManager.Repositories.Interfaces;
using PersonalFinanceManager.Services.Reports.Models;
using PersonalFinanceManager.Services.Reports.Templates;
using PersonalFinanceManager.Models.Enums;
using PersonalFinanceManager.Services.Interfaces;

namespace PersonalFinanceManager.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly IPendenciaRepository _pendenciaRepository;
        private readonly ICartaoCreditoRepository _cartaoRepository;
        private readonly IDashboardService _dashboardService;

        public ReportService(
            IPendenciaRepository pendenciaRepository,
            ICartaoCreditoRepository cartaoRepository,
            IDashboardService dashboardService)
        {
            _pendenciaRepository = pendenciaRepository;
            _cartaoRepository = cartaoRepository;
            _dashboardService = dashboardService;
        }

        public async Task<string> GerarRelatorioPendenciasAsync(ReportOptions options)
        {
            var pendencias = await _pendenciaRepository.GetAllAsync();

            // Filtros baseados em options
            if (options.FiltrarPorStatus.HasValue)
                pendencias = pendencias.Where(p => p.Status == options.FiltrarPorStatus.Value);

            if (options.DataInicio.HasValue)
                pendencias = pendencias.Where(p => p.DataCriacao >= options.DataInicio.Value);

            var outputDir = options.OutputDirectory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FinanceManager", "Reports");
            Directory.CreateDirectory(outputDir);
            
            // Nome do arquivo baseado no filtro
            var nomeArquivo = options.FiltrarPorStatus == StatusPendencia.Atrasada 
                ? $"Pendencias_Atrasadas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                : $"Todas_Pendencias_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            
            var outputPath = Path.Combine(outputDir, nomeArquivo);

            var document = new PendenciasReportTemplate(pendencias, options);
            document.GeneratePdf(outputPath);

            return outputPath;
        }

        public async Task<string> GerarRelatorioCartoesAsync(ReportOptions options)
        {
            var cartoes = await _cartaoRepository.GetAllAsync();
            var outputDir = options.OutputDirectory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FinanceManager", "Reports");
            Directory.CreateDirectory(outputDir);
            var outputPath = Path.Combine(outputDir, $"Resumo_Cartoes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            var document = new CartoesReportTemplate(cartoes, options);
            document.GeneratePdf(outputPath);
            return outputPath;
        }

        public async Task<string> GerarRelatorioDashboardAsync()
        {
            var resumo = await _dashboardService.ObterResumoAsync();

            var outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FinanceManager", "Reports");
            Directory.CreateDirectory(outputDir);
            var outputPath = Path.Combine(outputDir, $"Dashboard_Completo_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            var document = new DashboardReportTemplate(resumo);
            document.GeneratePdf(outputPath);

            return outputPath;
        }
    }
}
