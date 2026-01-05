using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceManager.Services.Import.Models;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Services.Import.Parsers
{
    public class ManualTextParser : ITextParser
    {
        public bool PodeProcessar(string texto)
        {
            return !string.IsNullOrWhiteSpace(texto);
        }

        public async Task<ImportResult> ParseAsync(string texto, ImportFormat formato)
        {
            var result = new ImportResult
            {
                DataImportacao = DateTime.Now,
                FormatoUtilizado = formato.Nome
            };

            try
            {
                if (formato.UsaChaveValor)
                    result.PendenciasImportadas = ParseFormatoChaveValor(texto, formato);
                else
                    result.PendenciasImportadas = ParseFormatoCsv(texto, formato);

                result.TotalImportados = result.PendenciasImportadas.Count;
            }
            catch (Exception ex)
            {
                result.Erros.Add($"Erro ao processar: {ex.Message}");
            }

            return result;
        }

        public IEnumerable<ImportedPendencia> ValidarEPreparar(ImportResult result)
        {
            // Retorna apenas as pendências válidas
            return result.PendenciasImportadas.Where(p => !string.IsNullOrWhiteSpace(p.Nome));
        }

        private List<ImportedPendencia> ParseFormatoChaveValor(string texto, ImportFormat formato)
        {
            var pendencias = new List<ImportedPendencia>();
            var registros = texto.Split(new[] { formato.DelimitadorRegistro }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var registro in registros)
            {
                var pendencia = new ImportedPendencia();
                var linhas = registro.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (var linha in linhas)
                {
                    var partes = linha.Split(formato.Separador, 2);
                    if (partes.Length != 2) continue;

                    var chave = partes[0].Trim().ToLowerInvariant();
                    var valor = partes[1].Trim();

                    switch (chave)
                    {
                        case "nome":
                            pendencia.Nome = valor;
                            break;
                        case "valor":
                            pendencia.Valor = ParseValor(valor);
                            break;
                        case "data":
                            pendencia.Data = ParseData(valor);
                            break;
                        case "prioridade":
                            pendencia.Prioridade = valor;
                            break;
                        case "status":
                            pendencia.Status = valor;
                            break;
                        case "tipo":
                            pendencia.Tipo = valor;
                            break;
                        case "cartao":
                        case "cartão":
                            pendencia.Cartao = valor;
                            break;
                        case "descricao":
                        case "descrição":
                            // Adicionar campo se necessário
                            break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(pendencia.Nome))
                    pendencias.Add(pendencia);
            }

            return pendencias;
        }

        private List<ImportedPendencia> ParseFormatoCsv(string texto, ImportFormat formato)
        {
            var pendencias = new List<ImportedPendencia>();
            var linhas = texto.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var linha in linhas)
            {
                if (linha.Trim().StartsWith("#")) continue; // Comentário

                var campos = linha.Split(formato.Separador)
                    .Select(c => c.Trim())
                    .ToArray();

                if (campos.Length < 3) continue; // Mínimo: nome, valor, data

                var pendencia = new ImportedPendencia
                {
                    Nome = campos[0],
                    Valor = ParseValor(campos[1]),
                    Data = campos.Length > 2 ? ParseData(campos[2]) : DateTime.MinValue,
                    Prioridade = campos.Length > 3 ? campos[3] : null,
                    Status = campos.Length > 4 ? campos[4] : null,
                    Tipo = campos.Length > 5 ? campos[5] : null,
                    Cartao = campos.Length > 6 ? campos[6] : null
                };

                pendencias.Add(pendencia);
            }

            return pendencias;
        }

        private decimal ParseValor(string valor)
        {
            decimal.TryParse(valor.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var result);
            return result;
        }

        private DateTime ParseData(string data)
        {
            DateTime.TryParse(data, new CultureInfo("pt-BR"), DateTimeStyles.None, out var result);
            return result;
        }
    }
}
