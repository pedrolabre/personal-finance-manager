using System.Collections.Generic;
namespace PersonalFinanceManager.Models.DTOs;

public class DashboardResumoDto
{
    public decimal TotalDividas { get; set; }
    public decimal TotalPago { get; set; }
    public decimal TotalRestante => TotalDividas - TotalPago;
    public int QuantidadePendencias { get; set; }
    public int QuantidadePendenciasAtrasadas { get; set; }
    public int QuantidadeParcelasProximosVencimentos { get; set; }
    public decimal ValorProximosVencimentos { get; set; }
    public decimal TotalRecebimentosEsperados { get; set; }
    public decimal TotalRecebimentosRecebidos { get; set; }
    public int QuantidadeRecebimentosAtrasados { get; set; }
    public double PercentualPago => TotalDividas > 0 ? (double)(TotalPago / TotalDividas * 100) : 0;
    public List<CartaoCreditoDto> ResumoCartoes { get; set; } = new();
    public List<ParcelaDto> ProximosVencimentos { get; set; } = new();
}
