using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceManager.Data.Entities;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Models.Enums;
using PersonalFinanceManager.Repositories.Interfaces;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;

namespace PersonalFinanceManager.Services.Implementations;

public class PendenciaService : IPendenciaService
{
    private readonly IPendenciaRepository _repository;
    private readonly ICartaoCreditoRepository _cartaoRepository;
    private readonly IParcelaRepository _parcelaRepository;
    private readonly IMessenger _messenger;
    
    public PendenciaService(
        IPendenciaRepository repository,
        ICartaoCreditoRepository cartaoRepository,
        IParcelaRepository parcelaRepository,
        IMessenger messenger)
    {
        _repository = repository;
        _cartaoRepository = cartaoRepository;
        _parcelaRepository = parcelaRepository;
        _messenger = messenger;
    }
    
    public async Task<IEnumerable<PendenciaDto>> ListarTodasAsync()
    {
        var pendencias = await _repository.GetAllAsync();
        return pendencias.Select(MapearParaDto);
    }
    
    public async Task<PendenciaDto> ObterPorIdAsync(int id)
    {
        var pendencia = await _repository.GetByIdAsync(id);
        return pendencia != null ? MapearParaDto(pendencia) : null;
    }
    
    public async Task<IEnumerable<PendenciaDto>> ListarPorStatusAsync(StatusPendencia status)
    {
        var pendencias = await _repository.GetByStatusAsync(status);
        return pendencias.Select(MapearParaDto);
    }
    
    public async Task<IEnumerable<PendenciaDto>> ListarAtradasAsync()
    {
        var pendencias = await _repository.GetAtradasAsync();
        return pendencias.Select(MapearParaDto);
    }
    
    public async Task<PendenciaDto> CriarAsync(PendenciaDto dto)
    {
        await ValidarPendenciaAsync(dto);
        
        var pendencia = new Pendencia
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            ValorTotal = dto.ValorTotal,
            DataCriacao = DateTime.Now,
            Prioridade = dto.Prioridade,
            Status = dto.Status,
            TipoDivida = dto.TipoDivida,
            CartaoCreditoId = dto.CartaoCreditoId,
            Parcelada = dto.Parcelada
        };
        
        var resultado = await _repository.AddAsync(pendencia);
        
        // Criar parcelas
        if (dto.DataVencimento.HasValue)
        {
            if (dto.Parcelada && dto.QuantidadeParcelas > 1)
            {
                // Criar múltiplas parcelas
                var valorParcela = Math.Round(dto.ValorTotal / dto.QuantidadeParcelas, 2);
                var intervalo = dto.IntervaloDiasParcelas > 0 ? dto.IntervaloDiasParcelas : 30;
                
                for (int i = 1; i <= dto.QuantidadeParcelas; i++)
                {
                    // Ajustar última parcela para compensar arredondamentos
                    var valorParcelaAtual = i == dto.QuantidadeParcelas
                        ? dto.ValorTotal - (valorParcela * (dto.QuantidadeParcelas - 1))
                        : valorParcela;
                    
                    var parcela = new Parcela
                    {
                        PendenciaId = resultado.Id,
                        AcordoId = null,
                        NumeroParcela = i,
                        Valor = valorParcelaAtual,
                        DataVencimento = dto.DataVencimento.Value.AddDays((i - 1) * intervalo),
                        Status = StatusParcela.Pendente
                    };
                    await _parcelaRepository.AddAsync(parcela);
                }
            }
            else
            {
                // Criar uma única parcela
                var parcela = new Parcela
                {
                    PendenciaId = resultado.Id,
                    AcordoId = null,
                    NumeroParcela = 1,
                    Valor = dto.ValorTotal,
                    DataVencimento = dto.DataVencimento.Value,
                    Status = StatusParcela.Pendente
                };
                await _parcelaRepository.AddAsync(parcela);
            }
        }
        
        _messenger.Send(new PendenciaCriadaMessage(resultado.Id));
        
        return MapearParaDto(resultado);
    }
    
    public async Task AtualizarAsync(int id, PendenciaDto dto)
    {
        var pendencia = await _repository.GetByIdAsync(id);
        if (pendencia == null)
            throw new InvalidOperationException("Pendência não encontrada");
        
        await ValidarPendenciaAsync(dto, id);
        
        pendencia.Nome = dto.Nome;
        pendencia.Descricao = dto.Descricao;
        pendencia.ValorTotal = dto.ValorTotal;
        pendencia.Prioridade = dto.Prioridade;
        pendencia.Status = dto.Status;
        pendencia.TipoDivida = dto.TipoDivida;
        pendencia.CartaoCreditoId = dto.CartaoCreditoId;
        pendencia.Parcelada = dto.Parcelada;
        
        await _repository.UpdateAsync(pendencia);
        
        // Recriar parcelas se a pendência for parcelada e tiver data de vencimento
        if (dto.Parcelada && dto.DataVencimento.HasValue && dto.QuantidadeParcelas > 0)
        {
            // Remover parcelas antigas
            var parcelasAntigas = await _parcelaRepository.GetByPendenciaAsync(id);
            foreach (var parcelaAntiga in parcelasAntigas)
            {
                await _parcelaRepository.DeleteAsync(parcelaAntiga.Id);
            }
            
            // Criar novas parcelas
            var valorPorParcela = dto.ValorTotal / dto.QuantidadeParcelas;
            var resto = dto.ValorTotal % dto.QuantidadeParcelas;
            
            for (int i = 1; i <= dto.QuantidadeParcelas; i++)
            {
                var dataVencimento = dto.DataVencimento.Value.AddDays((i - 1) * dto.IntervaloDiasParcelas);
                var valorParcela = valorPorParcela;
                
                // Adicionar o resto na última parcela
                if (i == dto.QuantidadeParcelas)
                {
                    valorParcela += resto;
                }
                
                var parcela = new Parcela
                {
                    PendenciaId = id,
                    AcordoId = null,
                    NumeroParcela = i,
                    Valor = valorParcela,
                    DataVencimento = dataVencimento,
                    Status = StatusParcela.Pendente
                };
                
                await _parcelaRepository.AddAsync(parcela);
            }
        }
        
        _messenger.Send(new PendenciaAtualizadaMessage(id));
    }
    
    public async Task QuitarAsync(int id)
    {
        var pendencia = await _repository.GetByIdAsync(id);
        if (pendencia == null)
            throw new InvalidOperationException("Pendência não encontrada");
        
        pendencia.Status = StatusPendencia.Quitada;
        await _repository.UpdateAsync(pendencia);
        _messenger.Send(new PendenciaAtualizadaMessage(id));
    }
    
    public async Task ExcluirAsync(int id)
    {
        await _repository.DeleteAsync(id);
        _messenger.Send(new PendenciaExcluidaMessage(id));
    }
    
    public async Task AtualizarStatusAsync(int id, StatusPendencia novoStatus)
    {
        var pendencia = await _repository.GetByIdAsync(id);
        if (pendencia == null)
            throw new InvalidOperationException("Pendência não encontrada");
        
        pendencia.Status = novoStatus;
        await _repository.UpdateAsync(pendencia);
        _messenger.Send(new PendenciaAtualizadaMessage(id));
    }
    
    private async Task ValidarPendenciaAsync(PendenciaDto dto, int? ignorarId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório");
        
        if (dto.ValorTotal <= 0)
            throw new ArgumentException("Valor deve ser maior que zero");
        
        if (dto.TipoDivida == TipoDivida.CartaoCredito && !dto.CartaoCreditoId.HasValue)
            throw new ArgumentException("Cartão de crédito deve ser informado para dívidas de cartão");
        
        if (dto.CartaoCreditoId.HasValue)
        {
            var cartao = await _cartaoRepository.GetByIdAsync(dto.CartaoCreditoId.Value);
            if (cartao == null)
                throw new ArgumentException("Cartão de crédito não encontrado");
        }
    }
    
    private PendenciaDto MapearParaDto(Pendencia pendencia)
    {
        var valorPago = pendencia.Parcelas
            .Where(p => p.Status == StatusParcela.Paga)
            .Sum(p => p.Valor);
        
        // Obter a primeira parcela pendente para a data de vencimento
        var primeiraParcela = pendencia.Parcelas
            .Where(p => p.Status == StatusParcela.Pendente)
            .OrderBy(p => p.DataVencimento)
            .FirstOrDefault();
        
        // Calcular intervalo entre parcelas (se houver pelo menos 2 parcelas)
        var parcelasOrdenadas = pendencia.Parcelas.OrderBy(p => p.DataVencimento).ToList();
        int intervaloDias = 30; // Padrão
        if (parcelasOrdenadas.Count >= 2)
        {
            var diff = (parcelasOrdenadas[1].DataVencimento - parcelasOrdenadas[0].DataVencimento).Days;
            if (diff > 0) intervaloDias = diff;
        }
        
        return new PendenciaDto
        {
            Id = pendencia.Id,
            Nome = pendencia.Nome,
            Descricao = pendencia.Descricao,
            ValorTotal = pendencia.ValorTotal,
            DataCriacao = pendencia.DataCriacao,
            DataVencimento = primeiraParcela?.DataVencimento,
            Prioridade = pendencia.Prioridade,
            Status = pendencia.Status,
            TipoDivida = pendencia.TipoDivida,
            CartaoCreditoId = pendencia.CartaoCreditoId,
            NomeCartao = pendencia.CartaoCredito?.Nome,
            Parcelada = pendencia.Parcelada,
            QuantidadeParcelas = pendencia.Parcelas.Count,
            IntervaloDiasParcelas = intervaloDias,
            ValorPago = valorPago
        };
    }
}
