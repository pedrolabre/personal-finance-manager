#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PersonalFinanceManager.Core.Commands;
using PersonalFinanceManager.Core.Dialogs;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;
using PersonalFinanceManager.Core.Navigation;

namespace PersonalFinanceManager.ViewModels.Base;

/// <summary>
/// Classe base para ViewModels de listagem
/// Fornece funcionalidade comum: carregar, filtrar, editar, excluir
/// </summary>
public abstract class BaseListViewModel<TDto> : ViewModelBase where TDto : class
{
    protected readonly INavigationService NavigationService;
    protected readonly IMessenger Messenger;
    protected readonly IDialogService DialogService;
    
    private bool _isLoading;
    private string _filtroTexto = string.Empty;
    
    public ObservableCollection<TDto> Items { get; }
    public ObservableCollection<TDto> ItemsFiltrados { get; }
    
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    
    public string FiltroTexto
    {
        get => _filtroTexto;
        set
        {
            if (SetProperty(ref _filtroTexto, value))
            {
                AplicarFiltro();
            }
        }
    }
    
    protected abstract string EntityName { get; }
    protected abstract string EntityNamePlural { get; }
    
    public ICommand NovoCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand ExcluirCommand { get; }
    public ICommand AtualizarCommand { get; }
    
    protected BaseListViewModel(
        INavigationService navigationService,
        IMessenger messenger,
        IDialogService dialogService)
    {
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        Messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        
        Items = new ObservableCollection<TDto>();
        ItemsFiltrados = new ObservableCollection<TDto>();
        
        NovoCommand = new RelayCommand(ExecuteNovo);
        EditarCommand = new RelayCommand<int?>(ExecuteEditar);
        ExcluirCommand = new AsyncRelayCommand<int?>(ExecuteExcluirAsync);
        AtualizarCommand = new AsyncRelayCommand(ExecuteAtualizarAsync);
        
        RegistrarMensagens();
    }
    
    public override void OnNavigatedTo(object? parameter = null)
    {
        base.OnNavigatedTo(parameter);
        _ = CarregarDadosAsync();
    }
    
    private async Task CarregarDadosAsync()
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
        catch (Exception ex)
        {
            DialogService.ShowError(
                $"Erro ao carregar {EntityNamePlural.ToLower()}: {ex.Message}",
                "Erro"
            );
            Messenger.Send(new ErrorMessage($"Erro ao carregar {EntityNamePlural.ToLower()}", ex));
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    protected virtual void AplicarFiltro()
    {
        ItemsFiltrados.Clear();
        
        var filtrados = string.IsNullOrWhiteSpace(FiltroTexto)
            ? Items
            : Items.Where(item => MatchFilter(item, FiltroTexto));
        
        foreach (var item in filtrados)
        {
            ItemsFiltrados.Add(item);
        }
    }
    
    private void ExecuteNovo()
    {
        NavigateToForm(null);
    }
    
    private void ExecuteEditar(int? id)
    {
        if (id.HasValue)
        {
            NavigateToForm(id.Value);
        }
    }
    
    private async Task ExecuteExcluirAsync(int? id)
    {
        if (!id.HasValue) return;
        
        var confirmar = DialogService.Confirm(
            $"Deseja realmente excluir este(a) {EntityName.ToLower()}?\n\nEsta ação não pode ser desfeita.",
            $"Excluir {EntityName}"
        );
        
        if (!confirmar) return;
        
        try
        {
            await DeleteAsync(id.Value);
            
            DialogService.ShowSuccess(
                $"{EntityName} excluído(a) com sucesso!",
                "Sucesso"
            );
            
            await CarregarDadosAsync();
        }
        catch (Exception ex)
        {
            DialogService.ShowError(
                $"Erro ao excluir {EntityName.ToLower()}: {ex.Message}",
                "Erro"
            );
            Messenger.Send(new ErrorMessage($"Erro ao excluir {EntityName.ToLower()}", ex));
        }
    }
    
    private async Task ExecuteAtualizarAsync()
    {
        await CarregarDadosAsync();
    }
    
    private void RegistrarMensagens()
    {
        // Recarregar quando houver alterações
        Messenger.Register<object>(this, async msg =>
        {
            if (ShouldReloadOnMessage(msg))
            {
                await CarregarDadosAsync();
            }
        });
    }
    
    /// <summary>
    /// Carrega os dados do repositório/serviço
    /// </summary>
    protected abstract Task<IEnumerable<TDto>> LoadDataAsync();
    
    /// <summary>
    /// Navega para o formulário de criação/edição
    /// </summary>
    protected abstract void NavigateToForm(int? id);
    
    /// <summary>
    /// Exclui o item do repositório/serviço
    /// </summary>
    protected abstract Task DeleteAsync(int id);
    
    /// <summary>
    /// Verifica se o item corresponde ao filtro
    /// </summary>
    protected abstract bool MatchFilter(TDto item, string filter);
    
    /// <summary>
    /// Determina se deve recarregar dados ao receber uma mensagem
    /// </summary>
    protected abstract bool ShouldReloadOnMessage(object message);
}
