#nullable enable
using System.Collections.Generic;
using PersonalFinanceManager.Services.Import.Models;

namespace PersonalFinanceManager.Services.Import.Parsers.Strategies
{
    /// <summary>
    /// Interface para estratégias de parsing de CSV
    /// Permite implementações específicas por instituição financeira
    /// </summary>
    public interface ICsvParsingStrategy
    {
        /// <summary>
        /// Nome da estratégia (ex: "Nubank", "Inter", "Generic")
        /// </summary>
        string StrategyName { get; }
        
        /// <summary>
        /// Verifica se esta estratégia pode processar o arquivo
        /// </summary>
        bool CanHandle(string csvContent, char separator);
        
        /// <summary>
        /// Parse das linhas CSV em objetos ImportedPendencia
        /// </summary>
        IEnumerable<ImportedPendencia> ParseLines(IEnumerable<string> lines, char separator, bool hasHeader);
        
        /// <summary>
        /// Detecta se o arquivo possui cabeçalho
        /// </summary>
        bool DetectHeader(string firstLine, char separator);
        
        /// <summary>
        /// Parse de uma linha individual
        /// </summary>
        ImportedPendencia? ParseLine(string line, char separator, Dictionary<string, int>? columnMapping = null);
    }
}
