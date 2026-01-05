#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalFinanceManager.Models.DTOs;

public class AcordoDto
{
    public int Id { get; set; }
    public int PendenciaId { get; set; }
    public string NomePendencia { get; set; } = string.Empty;
    public DateTime DataAcordo { get; set; }
    public int NumeroParcelas { get; set; }
    public decimal ValorTotal { get; set; }
    public string? Observacoes { get; set; }
    public bool Ativo { get; set; }
    public List<ParcelaDto> Parcelas { get; set; } = new();
    public int ParcelasPagas => Parcelas.Count(p => p.Status == Models.Enums.StatusParcela.Paga);
    public decimal ValorPago => Parcelas.Where(p => p.Status == Models.Enums.StatusParcela.Paga).Sum(p => p.Valor);
    public decimal ValorRestante => ValorTotal - ValorPago;
}
