#nullable enable
using System;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Data.Entities
{
    public class Parcela
    {
        public int Id { get; set; }
        public int NumeroParcela { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public StatusParcela Status { get; set; }
        public DateTime? DataPagamento { get; set; }

        public int PendenciaId { get; set; }
        public Pendencia? Pendencia { get; set; }

        public int? AcordoId { get; set; }
        public Acordo? Acordo { get; set; }
    }
}
