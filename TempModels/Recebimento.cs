using System;
using System.Collections.Generic;

namespace PersonalFinanceManager.TempModels;

public partial class Recebimento
{
    public int Id { get; set; }

    public string Descricao { get; set; }

    public int Categoria { get; set; }

    public DateTime DataPrevista { get; set; }

    public DateTime? DataRecebimento { get; set; }

    public int ValorEsperado { get; set; }

    public int ValorRecebido { get; set; }

    public int RecebimentoCompleto { get; set; }
}
