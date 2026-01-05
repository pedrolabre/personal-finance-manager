#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PersonalFinanceManager.Core.Commands;
using PersonalFinanceManager.Core.Dialogs;
using PersonalFinanceManager.Core.Logging;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;
using PersonalFinanceManager.Core.Navigation;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager.ViewModels.Acordos;

public class AcordoDetalhesViewModel : ViewModelBase
{
    private readonly IAcordoService _acordoService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly IMessenger _messenger;
    
    private int _acordoId;
    private AcordoDto? _acordo;
    private bool _isLoading;
    
    public AcordoDto? Acordo
    {
        get => _acordo;
        set => SetProperty(ref _acordo, value);
    }
    
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    
    public ICommand EditarCommand { get; }
    public ICommand ExcluirCommand { get; }
    public ICommand VoltarCommand { get; }
    
    public AcordoDetalhesViewModel(
        IAcordoService acordoService,
        INavigationService navigationService,
        IDialogService dialogService,
        IMessenger messenger)
    {
        _acordoService = acordoService ?? throw new ArgumentNullException(nameof(acordoService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        
        EditarCommand = new RelayCommand(ExecuteEditar);
        ExcluirCommand = new AsyncRelayCommand(ExecuteExcluirAsync);
        VoltarCommand = new RelayCommand(ExecuteVoltar);
    }
    
    public override void OnNavigatedTo(object? parameter = null)
    {
        base.OnNavigatedTo(parameter);
        
        if (parameter is int id)
        {
            _acordoId = id;
            DebugLogger.Log($"AcordoDetalhesViewModel - ID recebido: {id}");
            _ = CarregarDadosAsync();
        }
        else
        {
            DebugLogger.Log($"AcordoDetalhesViewModel - Parâmetro inválido: {parameter?.GetType().Name ?? "null"}");
        }
    }
    
    private async Task CarregarDadosAsync()
    {
        IsLoading = true;
        
        try
        {
            DebugLogger.Log($"AcordoDetalhesViewModel - Carregando acordo ID: {_acordoId}");
            Acordo = await _acordoService.ObterPorIdAsync(_acordoId);
            DebugLogger.Log($"AcordoDetalhesViewModel - Acordo carregado: {(Acordo != null ? Acordo.NomePendencia : "NULL")}");
            
            if (Acordo == null)
            {
                _messenger.Send(new ErrorMessage($"Acordo com ID {_acordoId} não encontrado", null));
            }
        }
        catch (Exception ex)
        {
            DebugLogger.Log($"AcordoDetalhesViewModel - Erro: {ex.Message}");
            _messenger.Send(new ErrorMessage("Erro ao carregar detalhes do acordo", ex));
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void ExecuteEditar()
    {
        _navigationService.NavigateTo<AcordoFormViewModel>(_acordoId);
    }
    
    private async Task ExecuteExcluirAsync()
    {
        var resultado = await _dialogService.ConfirmAsync(
            "Tem certeza que deseja excluir este acordo?",
            "Confirmar Exclusão");
        
        if (!resultado)
            return;
        
        try
        {
            await _acordoService.ExcluirAsync(_acordoId);
            _messenger.Send(new SuccessMessage("Acordo excluído com sucesso"));
            ExecuteVoltar();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Erro ao excluir acordo: {ex.Message}", "Erro");
            _messenger.Send(new ErrorMessage("Erro ao excluir acordo", ex));
        }
    }
    
    private void ExecuteVoltar()
    {
        _navigationService.NavigateTo<AcordosListViewModel>();
    }
}
