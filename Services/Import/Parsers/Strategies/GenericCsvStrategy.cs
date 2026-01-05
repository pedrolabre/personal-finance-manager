#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using PersonalFinanceManager.Services.Import.Models;

namespace PersonalFinanceManager.Services.Import.Parsers.Strategies
{
    /// <summary>
    /// Estratégia genérica para CSVs padrão
    /// </summary>
    public class GenericCsvStrategy : BaseCsvParsingStrategy
    {
        public override string StrategyName => "Generic CSV";
        
        public override bool CanHandle(string csvContent, char separator)
        {
            // Estratégia genérica sempre pode processar
            return true;
        }
        
        public override ImportedPendencia? ParseLine(string line, char separator, Dictionary<string, int>? columnMapping = null)
        {
            var fields = ParseCsvLine(line, separator);
            
            if (columnMapping != null)
                return ParseWithMapping(fields, columnMapping);
            
            return ParseByPosition(fields);
        }
        
        private ImportedPendencia ParseWithMapping(string[] fields, Dictionary<string, int> mapping)
        {
            var pendencia = new ImportedPendencia();
            
            if (mapping.TryGetValue("nome", out var nameIdx) && nameIdx < fields.Length)
                pendencia.Nome = fields[nameIdx];
            
            if (mapping.TryGetValue("valor", out var valueIdx) && valueIdx < fields.Length)
                pendencia.Valor = ParseValue(fields[valueIdx]);
            
            if (mapping.TryGetValue("data", out var dateIdx) && dateIdx < fields.Length)
                pendencia.Data = ParseDate(fields[dateIdx]);
            
            if (mapping.TryGetValue("prioridade", out var priorityIdx) && priorityIdx < fields.Length)
                pendencia.Prioridade = fields[priorityIdx];
            
            if (mapping.TryGetValue("status", out var statusIdx) && statusIdx < fields.Length)
                pendencia.Status = fields[statusIdx];
            
            if (mapping.TryGetValue("tipo", out var typeIdx) && typeIdx < fields.Length)
                pendencia.Tipo = fields[typeIdx];
            
            if (mapping.TryGetValue("cartao", out var cardIdx) && cardIdx < fields.Length)
                pendencia.Cartao = fields[cardIdx];
            
            return pendencia;
        }
        
        private ImportedPendencia ParseByPosition(string[] fields)
        {
            // Ordem padrão: Nome, Valor, Data, Prioridade, Status, Tipo, Cartão
            return new ImportedPendencia
            {
                Nome = fields.Length > 0 ? fields[0] : "",
                Valor = fields.Length > 1 ? ParseValue(fields[1]) : 0,
                Data = fields.Length > 2 ? ParseDate(fields[2]) : DateTime.MinValue,
                Prioridade = fields.Length > 3 ? fields[3].Trim() : null,
                Status = fields.Length > 4 ? fields[4].Trim() : null,
                Tipo = fields.Length > 5 ? fields[5].Trim() : null,
                Cartao = fields.Length > 6 ? fields[6].Trim() : null
            };
        }
    }
}
