#nullable enable
namespace PersonalFinanceManager.Models.DTOs;

public class CartaoCreditoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Banco { get; set; }
    public int DiaVencimento { get; set; }
    public int DiaFechamento { get; set; }
    public decimal? Limite { get; set; }
    public bool Ativo { get; set; }
    public decimal TotalDividas { get; set; }
    public int QuantidadeDividas { get; set; }
    public decimal? LimiteDisponivel => Limite.HasValue ? Limite.Value - TotalDividas : null;
    public double? PercentualUtilizado => Limite.HasValue && Limite.Value > 0 
        ? (double)(TotalDividas / Limite.Value * 100) 
        : null;
}
