#nullable enable
using System;
using System.Collections.Generic;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Data.Entities
{
    public class Pendencia
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime DataCriacao { get; set; }
        public Prioridade Prioridade { get; set; }
        public StatusPendencia Status { get; set; }
        public TipoDivida TipoDivida { get; set; }

        // Relacionamentos
        public int? CartaoCreditoId { get; set; }
        public CartaoCredito? CartaoCredito { get; set; }

        public bool Parcelada { get; set; }
        public ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();
        public ICollection<Acordo> Acordos { get; set; } = new List<Acordo>();
    }
}
