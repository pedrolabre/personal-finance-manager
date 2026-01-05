#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using PersonalFinanceManager.Core.Commands;
using PersonalFinanceManager.Core.Logging;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;
using PersonalFinanceManager.Core.Navigation;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager.ViewModels.Pendencias;

public class PendenciaDetalhesViewModel : ViewModelBase
{
    private readonly IPendenciaService _pendenciaService;
    private readonly IAcordoService _acordoService;
    private readonly INavigationService _navigationService;
    private readonly IMessenger _messenger;
    
    private int _pendenciaId;
    private PendenciaDto? _pendencia;
    private bool _isLoading;
    
    public PendenciaDto? Pendencia
    {
        get => _pendencia;
        set => SetProperty(ref _pendencia, value);
    }
    
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    
    public ObservableCollection<AcordoDto> Acordos { get; }
    
    public bool HasAcordos => Acordos?.Count > 0;
    
    public ICommand EditarCommand { get; }
    public ICommand ExcluirCommand { get; }
    public ICommand QuitarCommand { get; }
    public ICommand CriarAcordoCommand { get; }
    public ICommand VoltarCommand { get; }
    
    public PendenciaDetalhesViewModel(
        IPendenciaService pendenciaService,
        IAcordoService acordoService,
        INavigationService navigationService,
        IMessenger messenger)
    {
        _pendenciaService = pendenciaService ?? throw new ArgumentNullException(nameof(pendenciaService));
        _acordoService = acordoService ?? throw new ArgumentNullException(nameof(acordoService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        
        Acordos = new ObservableCollection<AcordoDto>();
        
        EditarCommand = new RelayCommand(ExecuteEditar);
        ExcluirCommand = new AsyncRelayCommand(ExecuteExcluirAsync);
        QuitarCommand = new AsyncRelayCommand(ExecuteQuitarAsync);
        CriarAcordoCommand = new RelayCommand(ExecuteCriarAcordo);
        VoltarCommand = new RelayCommand(ExecuteVoltar);
        
        _messenger.Register<PendenciaAtualizadaMessage>(this, msg =>
        {
            if (msg.PendenciaId == _pendenciaId)
            {
                _ = CarregarDadosAsync();
            }
        });
    }
    
    public override void OnNavigatedTo(object? parameter = null)
    {
        base.OnNavigatedTo(parameter);
        
        if (parameter is int id)
        {
            _pendenciaId = id;
            DebugLogger.Log($"PendenciaDetalhesViewModel - ID recebido: {id}");
            _ = CarregarDadosAsync();
        }
        else
        {
            DebugLogger.Log($"PendenciaDetalhesViewModel - Parâmetro inválido: {parameter?.GetType().Name ?? "null"}");
        }
    }
    
    private async Task CarregarDadosAsync()
    {
        IsLoading = true;
        
        try
        {
            DebugLogger.Log($"PendenciaDetalhesViewModel - Carregando pendência ID: {_pendenciaId}");
            Pendencia = await _pendenciaService.ObterPorIdAsync(_pendenciaId);
            DebugLogger.Log($"PendenciaDetalhesViewModel - Pendência carregada: {(Pendencia != null ? Pendencia.Nome : "NULL")}");
            
            if (Pendencia == null)
            {
                _messenger.Send(new ErrorMessage($"Pendência com ID {_pendenciaId} não encontrada", null));
            }
            
            var acordos = await _acordoService.ListarPorPendenciaAsync(_pendenciaId);
            Acordos.Clear();
            foreach (var acordo in acordos)
            {
                Acordos.Add(acordo);
            }
            OnPropertyChanged(nameof(HasAcordos));
        }
        catch (Exception ex)
        {
            DebugLogger.Log($"PendenciaDetalhesViewModel - Erro: {ex.Message}");
            _messenger.Send(new ErrorMessage("Erro ao carregar detalhes", ex));
        }
        finally
        {
            IsLoading = false;
            DebugLogger.Log($"PendenciaDetalhesViewModel - IsLoading setado para FALSE");
            DebugLogger.Log($"PendenciaDetalhesViewModel - Pendencia is null? {Pendencia == null}");
            if (Pendencia != null)
            {
                DebugLogger.Log($"PendenciaDetalhesViewModel - Pendencia.Nome = '{Pendencia.Nome}', ValorTotal = {Pendencia.ValorTotal}");
            }
        }
    }
    
    private void ExecuteEditar()
    {
        _navigationService.NavigateTo<PendenciaFormViewModel>(_pendenciaId);
    }
    
    private async Task ExecuteExcluirAsync()
    {
        var resultado = System.Windows.MessageBox.Show(
            "Tem certeza que deseja excluir esta pendência?\n\nEsta ação não pode ser desfeita.",
            "Confirmar Exclusão",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);
        
        if (resultado != System.Windows.MessageBoxResult.Yes)
            return;
        
        try
        {
            await _pendenciaService.ExcluirAsync(_pendenciaId);
            _messenger.Send(new SuccessMessage("Pendência excluída com sucesso"));
            _navigationService.NavigateBack();
        }
        catch (Exception ex)
        {
            _messenger.Send(new ErrorMessage("Erro ao excluir pendência", ex));
        }
    }
    
    private async Task ExecuteQuitarAsync()
    {
        try
        {
            await _pendenciaService.QuitarAsync(_pendenciaId);
            _messenger.Send(new SuccessMessage("Pendência quitada com sucesso"));
            await CarregarDadosAsync();
        }
        catch (Exception ex)
        {
            _messenger.Send(new ErrorMessage("Erro ao quitar pendência", ex));
        }
    }
    
    private void ExecuteCriarAcordo()
    {
        // Será implementado quando criarmos AcordoFormViewModel
        _messenger.Send(new WarningMessage("Funcionalidade de acordo será implementada em breve"));
    }
    
    private void ExecuteVoltar()
    {
        _navigationService.NavigateBack();
    }
}
