#nullable enable
using System.Collections.Generic;

namespace PersonalFinanceManager.Data.Entities
{
    public class CartaoCredito
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Banco { get; set; }
        public int DiaVencimento { get; set; }
        public int DiaFechamento { get; set; }
        public decimal? Limite { get; set; }
        public bool Ativo { get; set; }

        public ICollection<Pendencia> Pendencias { get; set; } = new List<Pendencia>();
    }
}
