using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Models.Enums;
using PersonalFinanceManager.Repositories.Interfaces;
using PersonalFinanceManager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceManager.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IPendenciaRepository _pendenciaRepository;
    private readonly IParcelaRepository _parcelaRepository;
    private readonly ICartaoCreditoRepository _cartaoRepository;
    private readonly IRecebimentoRepository _recebimentoRepository;
    
    public DashboardService(
        IPendenciaRepository pendenciaRepository,
        IParcelaRepository parcelaRepository,
        ICartaoCreditoRepository cartaoRepository,
        IRecebimentoRepository recebimentoRepository)
    {
        _pendenciaRepository = pendenciaRepository;
        _parcelaRepository = parcelaRepository;
        _cartaoRepository = cartaoRepository;
        _recebimentoRepository = recebimentoRepository;
    }
    
    public async Task<DashboardResumoDto> ObterResumoAsync()
    {
        var totalDividas = await _pendenciaRepository.GetTotalDividasAsync();
        var totalPago = await _pendenciaRepository.GetTotalPagoAsync();
        var quantidadeAtrasadas = await _pendenciaRepository.GetQuantidadeAtradasAsync();
        var proximosVencimentos = await ObterProximosVencimentosAsync(30); // Alterado de 7 para 30 dias
        var totalRecebimentosEsperados = await _recebimentoRepository.GetTotalEsperadoAsync();
        var totalRecebimentosRecebidos = await _recebimentoRepository.GetTotalRecebidoAsync();
        var recebimentosAtrasados = await _recebimentoRepository.GetAtrasadosAsync();
        var cartoes = await _cartaoRepository.GetAtivosAsync();
        var pendencias = await _pendenciaRepository.GetAllAsync();
        
        var resumoCartoes = cartoes.Select(c => new CartaoCreditoDto
        {
            Id = c.Id,
            Nome = c.Nome,
            Banco = c.Banco,
            DiaVencimento = c.DiaVencimento,
            DiaFechamento = c.DiaFechamento,
            Limite = c.Limite,
            Ativo = c.Ativo,
            TotalDividas = c.Pendencias.Where(p => p.Status != StatusPendencia.Quitada).Sum(p => p.ValorTotal),
            QuantidadeDividas = c.Pendencias.Count(p => p.Status != StatusPendencia.Quitada)
        }).ToList();
        
        return new DashboardResumoDto
        {
            TotalDividas = totalDividas,
            TotalPago = totalPago,
            QuantidadePendencias = pendencias.Count(),
            QuantidadePendenciasAtrasadas = quantidadeAtrasadas,
            QuantidadeParcelasProximosVencimentos = proximosVencimentos.Count(),
            ValorProximosVencimentos = proximosVencimentos.Sum(p => p.Valor),
            TotalRecebimentosEsperados = totalRecebimentosEsperados,
            TotalRecebimentosRecebidos = totalRecebimentosRecebidos,
            QuantidadeRecebimentosAtrasados = recebimentosAtrasados.Count(),
            ResumoCartoes = resumoCartoes,
            ProximosVencimentos = proximosVencimentos.ToList()
        };
    }
    
    public async Task<IEnumerable<ParcelaDto>> ObterProximosVencimentosAsync(int dias = 7)
    {
        var parcelas = await _parcelaRepository.GetProximosVencimentosAsync(dias);
        
        return parcelas.Select(p => new ParcelaDto
        {
            Id = p.Id,
            NumeroParcela = p.NumeroParcela,
            Valor = p.Valor,
            DataVencimento = p.DataVencimento,
            Status = p.Status,
            DataPagamento = p.DataPagamento,
            PendenciaId = p.PendenciaId,
            NomePendencia = p.Pendencia?.Nome ?? string.Empty,
            AcordoId = p.AcordoId
        });
    }
    
    public async Task AtualizarStatusPendenciasAsync()
    {
        var pendencias = await _pendenciaRepository.GetAllAsync();
        
        foreach (var pendencia in pendencias)
        {
            if (pendencia.Status == StatusPendencia.Quitada)
                continue;
            
            var parcelasAtrasadas = pendencia.Parcelas
                .Any(p => p.Status == StatusParcela.Pendente && p.DataVencimento < DateTime.Now);
            
            if (parcelasAtrasadas)
            {
                pendencia.Status = StatusPendencia.Atrasada;
                await _pendenciaRepository.UpdateAsync(pendencia);
            }
        }
        
        // Atualizar status das parcelas
        var parcelas = await _parcelaRepository.GetByStatusAsync(StatusParcela.Pendente);
        
        foreach (var parcela in parcelas)
        {
            if (parcela.DataVencimento < DateTime.Now)
            {
                parcela.Status = StatusParcela.Atrasada;
                await _parcelaRepository.UpdateAsync(parcela);
            }
        }
    }
}
