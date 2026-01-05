using System;
using System.Collections.Generic;

namespace PersonalFinanceManager.TempModels;

public partial class Parcela
{
    public int Id { get; set; }

    public int? AcordoId { get; set; }

    public DateTime? DataPagamento { get; set; }

    public DateTime DataVencimento { get; set; }

    public int NumeroParcela { get; set; }

    public int PendenciaId { get; set; }

    public int Status { get; set; }

    public int Valor { get; set; }

    public virtual Acordo Acordo { get; set; }

    public virtual Pendencia Pendencia { get; set; }
}
