namespace PersonalFinanceManager.Models
{
    public class AppSettings
    {
        public string CaminhoPadraoRelatorios { get; set; } = string.Empty;
        public bool ExibirGraficosRelatorios { get; set; } = true;
        public bool IncluirDetalhesRelatorios { get; set; } = true;
    }
}
