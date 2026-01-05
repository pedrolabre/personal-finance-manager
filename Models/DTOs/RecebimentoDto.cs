using System;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Models.DTOs;

public class RecebimentoDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public CategoriaRecebimento Categoria { get; set; }
    public DateTime DataPrevista { get; set; }
    public DateTime? DataRecebimento { get; set; }
    public decimal ValorEsperado { get; set; }
    public decimal ValorRecebido { get; set; }
    public bool RecebimentoCompleto { get; set; }
    public decimal ValorPendente => ValorEsperado - ValorRecebido;
    public bool Atrasado => !RecebimentoCompleto && DateTime.Now > DataPrevista;
    public int DiasAtraso => Atrasado ? (DateTime.Now - DataPrevista).Days : 0;
    public double PercentualRecebido => ValorEsperado > 0 ? (double)(ValorRecebido / ValorEsperado * 100) : 0;
}
