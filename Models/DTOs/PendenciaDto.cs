using System;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Models.DTOs;

public class PendenciaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataVencimento { get; set; }
    public Prioridade Prioridade { get; set; }
    public StatusPendencia Status { get; set; }
    public TipoDivida TipoDivida { get; set; }
    public int? CartaoCreditoId { get; set; }
    public string NomeCartao { get; set; }
    public bool Parcelada { get; set; }
    public int QuantidadeParcelas { get; set; }
    public int IntervaloDiasParcelas { get; set; } = 30; // Padrão: 30 dias (mensal)
    public decimal ValorPago { get; set; }
    public decimal ValorRestante => ValorTotal - ValorPago;
    public double PercentualPago => ValorTotal > 0 ? (double)(ValorPago / ValorTotal * 100) : 0;
}
