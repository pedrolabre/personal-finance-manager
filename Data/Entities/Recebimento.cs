using System;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Data.Entities
{
    public class Recebimento
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public CategoriaRecebimento Categoria { get; set; }
        public DateTime DataPrevista { get; set; }
        public DateTime? DataRecebimento { get; set; }
        public decimal ValorEsperado { get; set; }
        public decimal ValorRecebido { get; set; }
        public bool RecebimentoCompleto { get; set; }

        // Propriedades calculadas (não mapeadas)
        public decimal ValorPendente => ValorEsperado - ValorRecebido;
        public bool Atrasado => !RecebimentoCompleto && DateTime.Now > DataPrevista;
    }
}
