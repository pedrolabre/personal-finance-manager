#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using PersonalFinanceManager.Services.Import.Models;

namespace PersonalFinanceManager.Services.Import.Parsers.Strategies
{
    /// <summary>
    /// Estratégia específica para arquivos CSV do Banco Inter
    /// Formato típico: data,descricao,valor,saldo
    /// </summary>
    public class InterCsvStrategy : BaseCsvParsingStrategy
    {
        public override string StrategyName => "Banco Inter CSV";
        
        public override bool CanHandle(string csvContent, char separator)
        {
            // Verificar se parece com formato Inter
            var firstLine = csvContent.Split('\n').FirstOrDefault()?.ToLowerInvariant() ?? "";
            
            // Inter geralmente usa ponto e vírgula
            return separator == ';' && 
                   (firstLine.Contains("inter") || 
                    firstLine.Contains("saldo") && firstLine.Contains("descricao") ||
                    firstLine.Contains("balance") && firstLine.Contains("description"));
        }
        
        public override ImportedPendencia? ParseLine(string line, char separator, Dictionary<string, int>? columnMapping = null)
        {
            var fields = ParseCsvLine(line, separator);
            
            if (fields.Length < 3) return null;
            
            // Formato Inter típico: data, descrição, valor, saldo
            var valor = fields.Length > 2 ? ParseValue(fields[2]) : 0;
            
            return new ImportedPendencia
            {
                Data = fields.Length > 0 ? ParseDate(fields[0]) : DateTime.Now,
                Nome = fields.Length > 1 ? fields[1].Trim() : "",
                Valor = Math.Abs(valor), // Inter pode ter valores negativos
                Status = valor < 0 ? "Pago" : "Em Aberto",
                Prioridade = "Normal",
                Tipo = valor < 0 ? "Débito" : "Crédito",
                Cartao = "Banco Inter"
            };
        }
    }
}
