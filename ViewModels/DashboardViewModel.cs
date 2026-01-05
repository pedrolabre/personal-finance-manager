using System.Collections.ObjectModel;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.ViewModels.Base;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PersonalFinanceManager.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IMessenger _messenger;
    
    private decimal _totalDividas;
    private decimal _totalPago;
    private decimal _totalRestante;
    private int _quantidadePendencias;
    private int _quantidadeAtrasadas;
    private int _quantidadeProximosVencimentos;
    private decimal _valorProximosVencimentos;
    private decimal _totalRecebimentosEsperados;
    private decimal _totalRecebimentosRecebidos;
    private int _quantidadeRecebimentosAtrasados;
    private double _percentualPago;
    private bool _isLoading;
    
    public decimal TotalDividas
    {
        get => _totalDividas;
        set => SetProperty(ref _totalDividas, value);
    }
    
    public decimal TotalPago
    {
        get => _totalPago;
        set => SetProperty(ref _totalPago, value);
    }
    
    public decimal TotalRestante
    {
        get => _totalRestante;
        set => SetProperty(ref _totalRestante, value);
    }
    
    public int QuantidadePendencias
    {
        get => _quantidadePendencias;
        set => SetProperty(ref _quantidadePendencias, value);
    }
    
    public int QuantidadeAtrasadas
    {
        get => _quantidadeAtrasadas;
        set => SetProperty(ref _quantidadeAtrasadas, value);
    }
    
    public int QuantidadeProximosVencimentos
    {
        get => _quantidadeProximosVencimentos;
        set => SetProperty(ref _quantidadeProximosVencimentos, value);
    }
    
    public decimal ValorProximosVencimentos
    {
        get => _valorProximosVencimentos;
        set => SetProperty(ref _valorProximosVencimentos, value);
    }
    
    public decimal TotalRecebimentosEsperados
    {
        get => _totalRecebimentosEsperados;
        set => SetProperty(ref _totalRecebimentosEsperados, value);
    }
    
    public decimal TotalRecebimentosRecebidos
    {
        get => _totalRecebimentosRecebidos;
        set => SetProperty(ref _totalRecebimentosRecebidos, value);
    }
    
    public int QuantidadeRecebimentosAtrasados
    {
        get => _quantidadeRecebimentosAtrasados;
        set => SetProperty(ref _quantidadeRecebimentosAtrasados, value);
    }
    
    public double PercentualPago
    {
        get => _percentualPago;
        set => SetProperty(ref _percentualPago, value);
    }
    
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    
    public ObservableCollection<CartaoCreditoDto> ResumoCartoes { get; }
    public ObservableCollection<ParcelaDto> ProximosVencimentos { get; }
    
    public DashboardViewModel(IDashboardService dashboardService, IMessenger messenger)
    {
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        
        ResumoCartoes = new ObservableCollection<CartaoCreditoDto>();
        ProximosVencimentos = new ObservableCollection<ParcelaDto>();
        
        // Registrar mensagens para atualizar quando houver mudanças
        // Removido registro antigo para evitar conflito de handlers
        _messenger.Register<PendenciaCriadaMessage>(this, async _ => await CarregarDadosAsync());
        _messenger.Register<PendenciaAtualizadaMessage>(this, async _ => await CarregarDadosAsync());
        _messenger.Register<PendenciaExcluidaMessage>(this, async _ => await CarregarDadosAsync());
    }
    
    public override void OnNavigatedTo(object parameter = null)
    {
        base.OnNavigatedTo(parameter);
        _ = CarregarDadosAsync();
    }
    
    private async Task CarregarDadosAsync()
    {
        IsLoading = true;
        
        try
        {
            // Atualizar status antes de carregar
            await _dashboardService.AtualizarStatusPendenciasAsync();
            
            var resumo = await _dashboardService.ObterResumoAsync();
            
            TotalDividas = resumo.TotalDividas;
            TotalPago = resumo.TotalPago;
            TotalRestante = resumo.TotalRestante;
            QuantidadePendencias = resumo.QuantidadePendencias;
            QuantidadeAtrasadas = resumo.QuantidadePendenciasAtrasadas;
            QuantidadeProximosVencimentos = resumo.QuantidadeParcelasProximosVencimentos;
            ValorProximosVencimentos = resumo.ValorProximosVencimentos;
            TotalRecebimentosEsperados = resumo.TotalRecebimentosEsperados;
            TotalRecebimentosRecebidos = resumo.TotalRecebimentosRecebidos;
            QuantidadeRecebimentosAtrasados = resumo.QuantidadeRecebimentosAtrasados;
            PercentualPago = resumo.PercentualPago;
            
            ResumoCartoes.Clear();
            foreach (var cartao in resumo.ResumoCartoes)
            {
                ResumoCartoes.Add(cartao);
            }
            
            ProximosVencimentos.Clear();
            foreach (var parcela in resumo.ProximosVencimentos)
            {
                ProximosVencimentos.Add(parcela);
            }
        }
        catch (Exception ex)
        {
            _messenger.Send(new ErrorMessage("Erro ao carregar dados do dashboard", ex));
        }
        finally
        {
            IsLoading = false;
        }
    }
}
