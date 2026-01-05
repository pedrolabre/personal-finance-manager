using System;
using System.Collections.Generic;

namespace PersonalFinanceManager.TempModels;

public partial class Pendencia
{
    public int Id { get; set; }

    public string Nome { get; set; }

    public string Descricao { get; set; }

    public int ValorTotal { get; set; }

    public DateTime DataCriacao { get; set; }

    public int Prioridade { get; set; }

    public int Status { get; set; }

    public int TipoDivida { get; set; }

    public int? CartaoCreditoId { get; set; }

    public int Parcelada { get; set; }

    public virtual ICollection<Acordo> Acordos { get; set; } = new List<Acordo>();

    public virtual CartoesCredito CartaoCredito { get; set; }

    public virtual ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();
}
