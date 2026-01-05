#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PersonalFinanceManager.Data.Entities;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Models.Enums;
using PersonalFinanceManager.Repositories.Interfaces;
using PersonalFinanceManager.Services.Interfaces;

namespace PersonalFinanceManager.Services.Implementations;

public class AcordoService : IAcordoService
{
    private readonly IAcordoRepository _acordoRepository;
    private readonly IPendenciaRepository _pendenciaRepository;
    private readonly IParcelaRepository _parcelaRepository;
    
    public AcordoService(
        IAcordoRepository acordoRepository,
        IPendenciaRepository pendenciaRepository,
        IParcelaRepository parcelaRepository)
    {
        _acordoRepository = acordoRepository;
        _pendenciaRepository = pendenciaRepository;
        _parcelaRepository = parcelaRepository;
    }
    
    public async Task<IEnumerable<AcordoDto>> ListarTodosAsync()
    {
        var acordos = await _acordoRepository.GetAllAsync();
        return acordos.Select(MapearParaDto);
    }
    
    public async Task<IEnumerable<AcordoDto>> ListarPorPendenciaAsync(int pendenciaId)
    {
        var acordos = await _acordoRepository.GetByPendenciaAsync(pendenciaId);
        return acordos.Select(MapearParaDto);
    }
    
    public async Task<AcordoDto?> ObterPorIdAsync(int id)
    {
        var acordo = await _acordoRepository.GetByIdAsync(id);
        
        if (acordo == null)
        {
            PersonalFinanceManager.Core.Logging.DebugLogger.Log($"AcordoService.ObterPorIdAsync - Acordo não encontrado: {id}");
            return null;
        }
        
        PersonalFinanceManager.Core.Logging.DebugLogger.Log($"AcordoService.ObterPorIdAsync - Acordo encontrado: ID={acordo.Id}, PendenciaId={acordo.PendenciaId}");
        PersonalFinanceManager.Core.Logging.DebugLogger.Log($"AcordoService.ObterPorIdAsync - Pendencia é null? {acordo.Pendencia == null}");
        
        if (acordo.Pendencia != null)
        {
            PersonalFinanceManager.Core.Logging.DebugLogger.Log($"AcordoService.ObterPorIdAsync - Nome da Pendência: {acordo.Pendencia.Nome}");
        }
        
        var dto = MapearParaDto(acordo);
        PersonalFinanceManager.Core.Logging.DebugLogger.Log($"AcordoService.ObterPorIdAsync - DTO mapeado: NomePendencia={dto.NomePendencia}");
        
        return dto;
    }
    
    public async Task<AcordoDto> CriarAsync(AcordoDto dto)
    {
        int pendenciaId = dto.PendenciaId;
        
        // Se não há pendência vinculada, criar uma nova pendência para o acordo
        if (pendenciaId <= 0)
        {
            var novaPendencia = new Pendencia
            {
                Nome = dto.NomePendencia,
                Descricao = dto.Observacoes ?? string.Empty,
                ValorTotal = dto.ValorTotal,
                DataCriacao = DateTime.Now,
                Status = StatusPendencia.Acordada,
                TipoDivida = TipoDivida.Outros,
                Prioridade = Prioridade.Media,
                Parcelada = dto.NumeroParcelas > 1
            };
            
            var pendenciaCriada = await _pendenciaRepository.AddAsync(novaPendencia);
            pendenciaId = pendenciaCriada.Id;
        }
        else
        {
            await ValidarAcordoAsync(dto);
            
            // Desativar acordos anteriores
            await _acordoRepository.DesativarAcordosAnterioresAsync(pendenciaId);
            
            // Atualizar status da pendência
            var pendencia = await _pendenciaRepository.GetByIdAsync(pendenciaId);
            if (pendencia != null)
            {
                pendencia.Status = StatusPendencia.Acordada;
                await _pendenciaRepository.UpdateAsync(pendencia);
            }
        }
        
        var acordo = new Acordo
        {
            PendenciaId = pendenciaId,
            DataAcordo = DateTime.Now,
            NumeroParcelas = dto.NumeroParcelas,
            ValorTotal = dto.ValorTotal,
            Observacoes = dto.Observacoes,
            Ativo = dto.Ativo
        };
        
        var resultado = await _acordoRepository.AddAsync(acordo);
        
        // Gerar parcelas
        var parcelas = await GerarParcelasAsync(resultado.Id, dto.NumeroParcelas, dto.ValorTotal, DateTime.Now);
        
        return MapearParaDto(resultado);
    }
    
    public async Task AtualizarAsync(int id, AcordoDto dto)
    {
        var acordo = await _acordoRepository.GetByIdAsync(id);
        if (acordo == null)
            throw new InvalidOperationException("Acordo não encontrado");
        
        await ValidarAcordoAsync(dto, id);
        
        acordo.NumeroParcelas = dto.NumeroParcelas;
        acordo.ValorTotal = dto.ValorTotal;
        acordo.Observacoes = dto.Observacoes;
        
        await _acordoRepository.UpdateAsync(acordo);
    }
    
    public async Task ExcluirAsync(int id)
    {
        var acordo = await _acordoRepository.GetByIdAsync(id);
        if (acordo == null)
            throw new InvalidOperationException("Acordo não encontrado");
        
        // Excluir parcelas associadas
        var parcelas = await _parcelaRepository.GetByAcordoAsync(id);
        foreach (var parcela in parcelas)
        {
            await _parcelaRepository.DeleteAsync(parcela.Id);
        }
        
        await _acordoRepository.DeleteAsync(id);
        
        // Atualizar status da pendência
        var pendencia = await _pendenciaRepository.GetByIdAsync(acordo.PendenciaId);
        if (pendencia != null)
        {
            pendencia.Status = StatusPendencia.EmAberto;
            await _pendenciaRepository.UpdateAsync(pendencia);
        }
    }
    
    public async Task<List<ParcelaDto>> GerarParcelasAsync(int acordoId, int numeroParcelas, decimal valorTotal, DateTime dataInicio)
    {
        var acordo = await _acordoRepository.GetByIdAsync(acordoId);
        if (acordo == null)
            throw new InvalidOperationException("Acordo não encontrado");
        
        var valorParcela = Math.Round(valorTotal / numeroParcelas, 2);
        var diferenca = valorTotal - (valorParcela * numeroParcelas);
        
        var parcelas = new List<ParcelaDto>();
        
        for (int i = 1; i <= numeroParcelas; i++)
        {
            var valorParcelaAtual = valorParcela;
            
            // Ajustar a última parcela com a diferença de arredondamento
            if (i == numeroParcelas)
                valorParcelaAtual += diferenca;
            
            var parcela = new Parcela
            {
                PendenciaId = acordo.PendenciaId,
                AcordoId = acordoId,
                NumeroParcela = i,
                Valor = valorParcelaAtual,
                DataVencimento = dataInicio.AddMonths(i - 1),
                Status = StatusParcela.Pendente
            };
            
            var resultado = await _parcelaRepository.AddAsync(parcela);
            parcelas.Add(MapearParcelaParaDto(resultado));
        }
        
        return parcelas;
    }
    
    private async Task ValidarAcordoAsync(AcordoDto dto, int? ignorarId = null)
    {
        if (dto.PendenciaId <= 0)
            throw new ArgumentException("Pendência inválida");
        
        var pendencia = await _pendenciaRepository.GetByIdAsync(dto.PendenciaId);
        if (pendencia == null)
            throw new ArgumentException("Pendência não encontrada");
        
        if (dto.NumeroParcelas <= 0)
            throw new ArgumentException("Número de parcelas deve ser maior que zero");
        
        if (dto.ValorTotal <= 0)
            throw new ArgumentException("Valor total deve ser maior que zero");
    }
    
    private AcordoDto MapearParaDto(Acordo acordo)
    {
        return new AcordoDto
        {
            Id = acordo.Id,
            PendenciaId = acordo.PendenciaId,
            NomePendencia = acordo.Pendencia?.Nome ?? string.Empty,
            DataAcordo = acordo.DataAcordo,
            NumeroParcelas = acordo.NumeroParcelas,
            ValorTotal = acordo.ValorTotal,
            Observacoes = acordo.Observacoes,
            Ativo = acordo.Ativo,
            Parcelas = acordo.Parcelas.Select(MapearParcelaParaDto).ToList()
        };
    }
    
    private ParcelaDto MapearParcelaParaDto(Parcela parcela)
    {
        return new ParcelaDto
        {
            Id = parcela.Id,
            NumeroParcela = parcela.NumeroParcela,
            Valor = parcela.Valor,
            DataVencimento = parcela.DataVencimento,
            Status = parcela.Status,
            DataPagamento = parcela.DataPagamento,
            PendenciaId = parcela.PendenciaId,
            NomePendencia = parcela.Pendencia?.Nome ?? string.Empty,
            AcordoId = parcela.AcordoId
        };
    }
}
