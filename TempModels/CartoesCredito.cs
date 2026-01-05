using System;
using System.Collections.Generic;

namespace PersonalFinanceManager.TempModels;

public partial class CartoesCredito
{
    public int Id { get; set; }

    public int Ativo { get; set; }

    public string Banco { get; set; }

    public int DiaFechamento { get; set; }

    public int DiaVencimento { get; set; }

    public int? Limite { get; set; }

    public string Nome { get; set; }

    public virtual ICollection<Pendencia> Pendencia { get; set; } = new List<Pendencia>();
}
