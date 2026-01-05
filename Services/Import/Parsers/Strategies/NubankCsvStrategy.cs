#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using PersonalFinanceManager.Services.Import.Models;

namespace PersonalFinanceManager.Services.Import.Parsers.Strategies
{
    /// <summary>
    /// Estratégia específica para arquivos CSV do Nubank
    /// Formato típico: data,categoria,titulo,valor
    /// </summary>
    public class NubankCsvStrategy : BaseCsvParsingStrategy
    {
        public override string StrategyName => "Nubank CSV";
        
        public override bool CanHandle(string csvContent, char separator)
        {
            // Verificar se parece com formato Nubank
            var firstLine = csvContent.Split('\n').FirstOrDefault()?.ToLowerInvariant() ?? "";
            
            // Nubank geralmente usa vírgula e tem colunas específicas
            return separator == ',' && 
                   (firstLine.Contains("nubank") || 
                    firstLine.Contains("categoria") && firstLine.Contains("titulo") ||
                    firstLine.Contains("category") && firstLine.Contains("title"));
        }
        
        public override ImportedPendencia? ParseLine(string line, char separator, Dictionary<string, int>? columnMapping = null)
        {
            var fields = ParseCsvLine(line, separator);
            
            if (fields.Length < 3) return null;
            
            // Formato Nubank típico: data, categoria, titulo, valor
            return new ImportedPendencia
            {
                Data = fields.Length > 0 ? ParseDate(fields[0]) : DateTime.Now,
                Tipo = fields.Length > 1 ? fields[1].Trim() : "Outros",
                Nome = fields.Length > 2 ? fields[2].Trim() : "",
                Valor = fields.Length > 3 ? Math.Abs(ParseValue(fields[3])) : 0,
                Status = "Em Aberto",
                Prioridade = "Normal",
                Cartao = "Nubank"
            };
        }
    }
}
