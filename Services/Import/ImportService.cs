using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Services.Import.Models;
using PersonalFinanceManager.Services.Import.Parsers;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.Services.Import;

namespace PersonalFinanceManager.Services.Import
{
    public class ImportService : IImportService
    {
        private readonly IPendenciaService _pendenciaService;
        private readonly ICartaoCreditoService _cartaoService;
        private readonly ITextParser _textParser;

        public ImportService(
            IPendenciaService pendenciaService,
            ICartaoCreditoService cartaoService,
            ITextParser textParser)
        {
            _pendenciaService = pendenciaService;
            _cartaoService = cartaoService;
            _textParser = textParser;
        }


        public async Task<ImportResult> ImportarAsync(string texto, ImportFormat formato)
        {
            var result = await _textParser.ParseAsync(texto, formato);

            if (result == null || result.PendenciasImportadas == null)
                return result;

            // Validar e preparar
            var pendenciasValidas = _textParser.ValidarEPreparar(result).ToList();

            // Resolver cartões
            await ResolverCartoesAsync(pendenciasValidas);

            // Importar
            foreach (var importada in pendenciasValidas)
            {
                try
                {
                    var dto = MapearParaDto(importada);
                    await _pendenciaService.CriarAsync(dto);
                    result.TotalImportados++;
                }
                catch (Exception ex)
                {
                    result.Erros.Add($"Erro ao importar '{importada.Nome}': {ex.Message}");
                    result.TotalFalhas++;
                }
            }

            return result;
        }

        // Implementação dos métodos esperados pela ViewModel
        public async Task<ImportResult> ValidarTextoAsync(string texto, ImportFormat formato)
        {
            var result = await _textParser.ParseAsync(texto, formato);
            if (result == null)
            {
                return new ImportResult { Sucesso = false, Erros = new List<string> { "Falha ao processar texto." } };
            }
            // Validar e preparar
            var pendenciasValidas = _textParser.ValidarEPreparar(result).ToList();
            result.Pendencias = pendenciasValidas;
            result.Sucesso = pendenciasValidas.Any() && !result.Erros.Any();
            result.TotalRegistros = result.PendenciasImportadas?.Count ?? 0;
            result.RegistrosImportados = pendenciasValidas.Count;
            return result;
        }

        public async Task<ImportResult> ImportarDeTextoAsync(string texto, ImportFormat formato)
        {
            // Reutiliza a lógica de ImportarAsync
            return await ImportarAsync(texto, formato);
        }

        public async Task<ImportResult> ImportarDeCsvAsync(string caminhoArquivo)
        {
            var conteudo = await File.ReadAllTextAsync(caminhoArquivo);
            return await ImportarAsync(conteudo, ImportFormat.FormatoCsv);
        }

        public async Task<ImportResult> ImportarDeExtratoAsync(string caminhoArquivo)
        {
            // Parser específico para extratos bancários (OFX, CSV de bancos)
            // Implementação futura
            throw new NotImplementedException("Importação de extratos em desenvolvimento");
        }

        private async Task ResolverCartoesAsync(List<ImportedPendencia> pendencias)
        {
            var cartoesExistentes = await _cartaoService.ListarTodosAsync();

            foreach (var pendencia in pendencias)
            {
                if (string.IsNullOrWhiteSpace(pendencia.Cartao))
                    continue;

                var cartao = cartoesExistentes.FirstOrDefault(c =>
                    c.Nome.Equals(pendencia.Cartao, StringComparison.OrdinalIgnoreCase));

                if (cartao != null)
                {
                    pendencia.CartaoCreditoId = cartao.Id;
                }
                else
                {
                    // Adicionar aviso se necessário
                }
            }
        }

        private PendenciaDto MapearParaDto(ImportedPendencia importada)
        {
            return new PendenciaDto
            {
                Nome = importada.Nome,
                Descricao = importada.Descricao,
                ValorTotal = importada.Valor,
                DataCriacao = importada.Data != default ? importada.Data : DateTime.Now,
                Prioridade = Enum.TryParse(importada.Prioridade, out PersonalFinanceManager.Models.Enums.Prioridade prioridade) ? prioridade : PersonalFinanceManager.Models.Enums.Prioridade.Media,
                Status = Enum.TryParse(importada.Status, out PersonalFinanceManager.Models.Enums.StatusPendencia status) ? status : PersonalFinanceManager.Models.Enums.StatusPendencia.EmAberto,
                TipoDivida = Enum.TryParse(importada.Tipo, out PersonalFinanceManager.Models.Enums.TipoDivida tipo) ? tipo : PersonalFinanceManager.Models.Enums.TipoDivida.Outros,
                CartaoCreditoId = importada.CartaoCreditoId
            };
        }
    }
}
