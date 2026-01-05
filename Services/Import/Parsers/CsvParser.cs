#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceManager.Services.Import.Models;
using PersonalFinanceManager.Services.Import.Parsers.Strategies;

namespace PersonalFinanceManager.Services.Import.Parsers
{
    /// <summary>
    /// Parser CSV refatorado usando Strategy Pattern
    /// Delega parsing específico para estratégias plugáveis
    /// </summary>
    public class CsvParser : ITextParser
    {
        private readonly CsvStrategyFactory _strategyFactory;
        private static readonly char[] PossibleSeparators = { ';', ',', '\t', '|' };

        public CsvParser(CsvStrategyFactory strategyFactory)
        {
            _strategyFactory = strategyFactory ?? throw new ArgumentNullException(nameof(strategyFactory));
        }

        public IEnumerable<ImportedPendencia> Parse(string input)
        {
            var formato = DetectFormat(input);
            var result = ParseCsv(input, formato);
            return result.Pendencias;
        }

        public bool PodeProcessar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            var lines = texto.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            return lines.Length >= 1 && DetectSeparator(texto) != '\0';
        }

        public Task<ImportResult> ParseAsync(string texto, ImportFormat formato)
        {
            return Task.FromResult(ParseCsv(texto, formato));
        }

        public IEnumerable<ImportedPendencia> ValidarEPreparar(ImportResult result)
        {
            if (result?.Pendencias == null)
                yield break;

            foreach (var pendencia in result.Pendencias)
            {
                if (string.IsNullOrWhiteSpace(pendencia.Nome))
                    continue;

                if (pendencia.Valor < 0)
                    pendencia.Valor = Math.Abs(pendencia.Valor);

                if (pendencia.Data == DateTime.MinValue)
                    pendencia.Data = DateTime.Now;

                pendencia.Status ??= "Em Aberto";
                pendencia.Prioridade ??= "Normal";

                yield return pendencia;
            }
        }

        private ImportResult ParseCsv(string texto, ImportFormat formato)
        {
            var result = new ImportResult
            {
                DataImportacao = DateTime.Now,
                FormatoUtilizado = formato?.Nome ?? "CSV Auto-detectado"
            };

            try
            {
                var separator = formato?.Separador ?? DetectSeparator(texto);
                if (separator == '\0')
                {
                    result.Erros.Add("Não foi possível detectar o separador do arquivo CSV");
                    return result;
                }

                var strategy = _strategyFactory.SelectStrategy(texto, separator);
                result.FormatoUtilizado = $"{strategy.StrategyName} ({separator})";

                var lines = texto.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var firstLine = lines.FirstOrDefault() ?? "";
                var hasHeader = strategy.DetectHeader(firstLine, separator);
                
                var linesToProcess = hasHeader ? lines.Skip(1) : lines;

                foreach (var line in linesToProcess)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                        continue;

                    result.TotalRegistros++;

                    try
                    {
                        var pendencia = strategy.ParseLine(line, separator, null);
                        
                        if (pendencia != null && !string.IsNullOrWhiteSpace(pendencia.Nome))
                        {
                            result.Pendencias.Add(pendencia);
                        }
                        else
                        {
                            result.Avisos.Add($"Linha ignorada: {line.Substring(0, Math.Min(50, line.Length))}");
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Erros.Add($"Erro na linha: {ex.Message}");
                        result.TotalFalhas++;
                    }
                }

                result.Sucesso = result.Pendencias.Any();
            }
            catch (Exception ex)
            {
                result.Erros.Add($"Erro geral: {ex.Message}");
            }

            return result;
        }

        private char DetectSeparator(string texto)
        {
            var firstLine = texto.Split('\n').FirstOrDefault() ?? "";
            
            var counts = PossibleSeparators
                .Select(s => new { Separator = s, Count = firstLine.Count(c => c == s) })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToList();

            foreach (var item in counts)
            {
                var lines = texto.Split('\n').Take(5).ToList();
                var distinctCounts = lines.Select(l => l.Count(c => c == item.Separator)).Distinct().Count();
                
                if (distinctCounts <= 2)
                    return item.Separator;
            }

            return counts.FirstOrDefault()?.Separator ?? '\0';
        }

        private ImportFormat DetectFormat(string texto)
        {
            var separator = DetectSeparator(texto);
            return new ImportFormat
            {
                Nome = "CSV Auto-detectado",
                Separador = separator,
                UsaChaveValor = false,
                DelimitadorRegistro = "\n"
            };
        }
    }
}
