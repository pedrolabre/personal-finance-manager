#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using PersonalFinanceManager.Services.Import.Models;

namespace PersonalFinanceManager.Services.Import.Parsers.Strategies
{
    /// <summary>
    /// Classe base abstrata com funcionalidades comuns de parsing
    /// </summary>
    public abstract class BaseCsvParsingStrategy : ICsvParsingStrategy
    {
        protected static readonly string[] DateFormats = new[]
        {
            "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "yyyy-MM-dd",
            "MM/dd/yyyy", "dd.MM.yyyy", "dd/MM/yy", "d/M/yy"
        };

        public abstract string StrategyName { get; }
        
        public abstract bool CanHandle(string csvContent, char separator);
        
        public virtual IEnumerable<ImportedPendencia> ParseLines(IEnumerable<string> lines, char separator, bool hasHeader)
        {
            var linesList = lines.ToList();
            if (!linesList.Any()) yield break;
            
            var columnMapping = hasHeader ? MapColumns(linesList.First(), separator) : null;
            var dataLines = hasHeader ? linesList.Skip(1) : linesList;
            
            foreach (var line in dataLines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                    continue;
                    
                var pendencia = ParseLine(line, separator, columnMapping);
                if (pendencia != null && !string.IsNullOrWhiteSpace(pendencia.Nome))
                    yield return pendencia;
            }
        }
        
        public virtual bool DetectHeader(string firstLine, char separator)
        {
            var fields = firstLine.Split(separator).Select(f => f.Trim().ToLowerInvariant()).ToArray();
            
            var headerKeywords = new[] { "nome", "valor", "data", "descricao", "descrição", 
                "vencimento", "status", "tipo", "cartao", "cartão", "prioridade", 
                "categoria", "parcela", "name", "value", "date", "description", "amount" };
            
            var matches = fields.Count(f => headerKeywords.Any(k => f.Contains(k)));
            return matches >= 2;
        }
        
        public abstract ImportedPendencia? ParseLine(string line, char separator, Dictionary<string, int>? columnMapping = null);
        
        protected virtual Dictionary<string, int> MapColumns(string headerLine, char separator)
        {
            var fields = headerLine.Split(separator).Select(f => f.Trim().ToLowerInvariant()).ToArray();
            var mapping = new Dictionary<string, int>();
            
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                
                if (field.Contains("nome") || field.Contains("name") || field.Contains("descri"))
                    mapping["nome"] = i;
                else if (field.Contains("valor") || field.Contains("value") || field.Contains("amount") || field.Contains("preco"))
                    mapping["valor"] = i;
                else if (field.Contains("data") || field.Contains("date") || field.Contains("vencimento"))
                    mapping["data"] = i;
                else if (field.Contains("prioridade") || field.Contains("priority"))
                    mapping["prioridade"] = i;
                else if (field.Contains("status") || field.Contains("situacao"))
                    mapping["status"] = i;
                else if (field.Contains("tipo") || field.Contains("type") || field.Contains("categoria"))
                    mapping["tipo"] = i;
                else if (field.Contains("cartao") || field.Contains("cartão") || field.Contains("card"))
                    mapping["cartao"] = i;
            }
            
            return mapping;
        }
        
        protected string[] ParseCsvLine(string line, char separator)
        {
            var fields = new List<string>();
            var currentField = "";
            var insideQuotes = false;
            
            foreach (var c in line)
            {
                if (c == '"')
                    insideQuotes = !insideQuotes;
                else if (c == separator && !insideQuotes)
                {
                    fields.Add(currentField.Trim());
                    currentField = "";
                }
                else
                    currentField += c;
            }
            fields.Add(currentField.Trim());
            
            return fields.ToArray();
        }
        
        protected decimal ParseValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;
            
            value = Regex.Replace(value, @"[R$\s€$£¥]", "").Trim();
            
            var hasComma = value.Contains(',');
            var hasDot = value.Contains('.');
            
            if (hasComma && hasDot)
            {
                var lastComma = value.LastIndexOf(',');
                var lastDot = value.LastIndexOf('.');
                
                value = lastComma > lastDot
                    ? value.Replace(".", "").Replace(",", ".")
                    : value.Replace(",", "");
            }
            else if (hasComma)
            {
                var parts = value.Split(',');
                value = parts.Length == 2 && parts[1].Length <= 2
                    ? value.Replace(",", ".")
                    : value.Replace(",", "");
            }
            
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
                ? result
                : 0;
        }
        
        protected DateTime ParseDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
                return DateTime.MinValue;
            
            date = date.Trim();
            
            foreach (var format in DateFormats)
            {
                if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    return result;
            }
            
            if (DateTime.TryParse(date, new CultureInfo("pt-BR"), DateTimeStyles.None, out var resultBr))
                return resultBr;
            
            return DateTime.MinValue;
        }
    }
}
