using System;
namespace PersonalFinanceManager.Services.Import.Models
{
    public class ImportedPendencia
    {
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string Prioridade { get; set; }
        public string Status { get; set; }
        public string Tipo { get; set; }
        public string Cartao { get; set; }
        public int? CartaoCreditoId { get; set; }
        public string Descricao { get; set; }
    }
}
