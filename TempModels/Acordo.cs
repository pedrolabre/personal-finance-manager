using System;
using System.Collections.Generic;

namespace PersonalFinanceManager.TempModels;

public partial class Acordo
{
    public int Id { get; set; }

    public int PendenciaId { get; set; }

    public DateTime DataAcordo { get; set; }

    public int NumeroParcelas { get; set; }

    public int ValorTotal { get; set; }

    public string Observacoes { get; set; }

    public int Ativo { get; set; }

    public virtual ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();

    public virtual Pendencia Pendencia { get; set; }
}
