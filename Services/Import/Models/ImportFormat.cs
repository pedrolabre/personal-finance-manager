namespace PersonalFinanceManager.Services.Import.Models
{
    public class ImportFormat
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Exemplo { get; set; }
        public char Separador { get; set; }
        public bool UsaChaveValor { get; set; }
        public string DelimitadorRegistro { get; set; }

        public static ImportFormat FormatoSimples => new()
        {
            Nome = "Formato Simples (Chave: Valor)",
            Descricao = "Cada campo em uma linha, registros separados por ---",
            Separador = ':',
            UsaChaveValor = true,
            DelimitadorRegistro = "---",
            Exemplo = @"Nome: Conta de luz\nValor: 150.50\nData: 25/12/2024\nPrioridade: Alta\n---"
        };

        public static ImportFormat FormatoCsv => new()
        {
            Nome = "CSV/Lista (Ponto e vírgula)",
            Descricao = "Uma linha por registro, campos separados por ;",
            Separador = ';',
            UsaChaveValor = false,
            DelimitadorRegistro = "\n",
            Exemplo = "Conta de luz; 150.50; 25/12/2024; Alta; Em Aberto"
        };

        public static ImportFormat FormatoJson => new()
        {
            Nome = "JSON",
            Descricao = "Array de objetos JSON",
            Exemplo = "[\n  {\"nome\": \"Conta de luz\", \"valor\": 150.50}\n]"
        };
    }
}
