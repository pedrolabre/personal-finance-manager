
using System;
using System.Collections.Generic;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Services.Import.Models
{
    public class ImportResult
    {
        public bool Sucesso { get; set; }
        public DateTime DataImportacao { get; set; }
        public string FormatoUtilizado { get; set; }
        public int TotalRegistros { get; set; }
        public int RegistrosImportados { get; set; }
        public int RegistrosFalhos { get; set; }
        public List<string> Erros { get; set; } = new();
        public List<string> Avisos { get; set; } = new();
        public List<ImportedPendencia> Pendencias { get; set; } = new();

        // For compatibility with legacy code
        public List<ImportedPendencia> PendenciasImportadas { get => Pendencias; set => Pendencias = value; }
        public int TotalImportados { get => RegistrosImportados; set => RegistrosImportados = value; }
        public int TotalFalhas { get => RegistrosFalhos; set => RegistrosFalhos = value; }

        public string ResumoImportacao =>
            $"Importados: {RegistrosImportados}/{TotalRegistros} | " +
            $"Falhas: {RegistrosFalhos} | " +
            $"Avisos: {Avisos.Count}";
    }

    // Removido: definição duplicada de ImportedPendencia. Utilize a classe de ImportedPendencia.cs
}
