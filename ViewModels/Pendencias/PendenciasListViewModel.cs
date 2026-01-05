#nullable enable
using System;
using System.Collections.Generic;
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

namespace PersonalFinanceManager.ViewModels.Pendencias;

public class PendenciasListViewModel : BaseListViewModel<PendenciaDto>
{
    private readonly IPendenciaService _pendenciaService;
    
    protected override string EntityName => "Pendência";
    protected override string EntityNamePlural => "Pendências";
    
    // Alias para compatibilidade com View
    public System.Collections.ObjectModel.ObservableCollection<PendenciaDto> PendenciasFiltradas => ItemsFiltrados;
    public ICommand NovaPendenciaCommand => NovoCommand;
    
    public ICommand VisualizarCommand { get; }
    public ICommand QuitarCommand { get; }
    
    public PendenciasListViewModel(
        IPendenciaService pendenciaService,
        INavigationService navigationService,
        IMessenger messenger,
        IDialogService dialogService)
        : base(navigationService, messenger, dialogService)
    {
        _pendenciaService = pendenciaService ?? throw new ArgumentNullException(nameof(pendenciaService));
        
        VisualizarCommand = new RelayCommand<int?>(ExecuteVisualizar);
        QuitarCommand = new AsyncRelayCommand<int?>(ExecuteQuitarAsync);
    }
    
    protected override async Task<IEnumerable<PendenciaDto>> LoadDataAsync()
    {
        return await _pendenciaService.ListarTodasAsync();
    }
    
    protected override void NavigateToForm(int? id)
    {
        if (id.HasValue)
            NavigationService.NavigateTo<PendenciaFormViewModel>(id.Value);
        else
            NavigationService.NavigateTo<PendenciaFormViewModel>();
    }
    
    protected override async Task DeleteAsync(int id)
    {
        await _pendenciaService.ExcluirAsync(id);
        Messenger.Send(new PendenciaExcluidaMessage(id));
    }
    
    protected override bool MatchFilter(PendenciaDto item, string filter)
    {
        return item.Nome.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
               (item.Descricao?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false);
    }
    
    protected override bool ShouldReloadOnMessage(object message)
    {
        return message is PendenciaCriadaMessage or 
               PendenciaAtualizadaMessage or 
               PendenciaExcluidaMessage;
    }
    
    private void ExecuteVisualizar(int? id)
    {
        DebugLogger.Log($"PendenciasListViewModel - ExecuteVisualizar chamado com ID: {id}");
        if (id.HasValue)
        {
            DebugLogger.Log($"PendenciasListViewModel - Navegando para PendenciaDetalhesViewModel com ID: {id.Value}");
            NavigationService.NavigateTo<PendenciaDetalhesViewModel>(id.Value);
        }
        else
        {
            DebugLogger.Log("PendenciasListViewModel - ID é null, não navegando");
        }
    }
    
    private async Task ExecuteQuitarAsync(int? id)
    {
        if (!id.HasValue)
            return;
        
        try
        {
            await _pendenciaService.QuitarAsync(id.Value);
            Messenger.Send(new SuccessMessage("Pendência quitada com sucesso"));
            Messenger.Send(new PendenciaAtualizadaMessage(id.Value));
            await RecarregarAsync(); // Recarregar para atualizar visualização
        }
        catch (Exception ex)
        {
            DialogService.ShowError($"Erro ao quitar pendência: {ex.Message}", "Erro");
            Messenger.Send(new ErrorMessage("Erro ao quitar pendência", ex));
        }
    }
    
    private async Task RecarregarAsync()
    {
        IsLoading = true;
        try
        {
            var dados = await LoadDataAsync();
            Items.Clear();
            foreach (var item in dados)
            {
                Items.Add(item);
            }
            AplicarFiltro();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
