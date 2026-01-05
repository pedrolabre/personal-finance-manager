
using System;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Models.DTOs;

public class ParcelaDto
{
    public int Id { get; set; }
    public int NumeroParcela { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public StatusParcela Status { get; set; }
    public DateTime? DataPagamento { get; set; }
    public int PendenciaId { get; set; }
    public string NomePendencia { get; set; } = string.Empty;
    public int? AcordoId { get; set; }
    public bool Atrasada => Status == StatusParcela.Pendente && DataVencimento < DateTime.Now;
    public int DiasAtraso => Atrasada ? (DateTime.Now - DataVencimento).Days : 0;
}
