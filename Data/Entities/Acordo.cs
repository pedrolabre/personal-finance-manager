#nullable enable
using System;
using System.Collections.Generic;

namespace PersonalFinanceManager.Data.Entities
{
    public class Acordo
    {
        public int Id { get; set; }
        public int PendenciaId { get; set; }
        public Pendencia? Pendencia { get; set; }

        public DateTime DataAcordo { get; set; }
        public int NumeroParcelas { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
        public bool Ativo { get; set; } // Permite histórico

        public ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();
    }
}
