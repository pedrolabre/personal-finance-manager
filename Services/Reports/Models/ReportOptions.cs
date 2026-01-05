using System;
using System.IO;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Services.Reports.Models
{
    public class ReportOptions
    {
        public string OutputDirectory { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public StatusPendencia? FiltrarPorStatus { get; set; }
        public Prioridade? FiltrarPorPrioridade { get; set; }
        public int? CartaoCreditoId { get; set; }
        public bool IncluirGraficos { get; set; }
        public bool IncluirDetalhes { get; set; }
        public TipoRelatorio TipoRelatorio { get; set; }

        public static ReportOptions Default => new()
        {
            OutputDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "FinanceManager",
                "Reports"),
            IncluirGraficos = true,
            IncluirDetalhes = true
        };
    }

    public enum TipoRelatorio
    {
        TodasPendencias,
        PendenciasAtrasadas,
        ResumoCartoes,
        DashboardCompleto
    }
}
