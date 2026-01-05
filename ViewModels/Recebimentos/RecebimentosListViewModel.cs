#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PersonalFinanceManager.Core.Commands;
using PersonalFinanceManager.Core.Dialogs;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;
using PersonalFinanceManager.Core.Navigation;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager.ViewModels.Recebimentos;

public class RecebimentosListViewModel : BaseListViewModel<RecebimentoDto>
{
    private readonly IRecebimentoService _recebimentoService;
    private bool _mostrarApenasPendentes;
    
    protected override string EntityName => "Recebimento";
    protected override string EntityNamePlural => "Recebimentos";
    
    // Alias para compatibilidade com View
    public System.Collections.ObjectModel.ObservableCollection<RecebimentoDto> Recebimentos => ItemsFiltrados;
    public ICommand NovoRecebimentoCommand => NovoCommand;
    
    public bool MostrarApenasPendentes
    {
        get => _mostrarApenasPendentes;
        set
        {
            if (SetProperty(ref _mostrarApenasPendentes, value))
            {
                _ = RecarregarAsync();
            }
        }
    }
    
    public ICommand RegistrarRecebimentoParcialCommand { get; }
    public ICommand RegistrarRecebimentoCompletoCommand { get; }
    
    public RecebimentosListViewModel(
        IRecebimentoService recebimentoService,
        INavigationService navigationService,
        IMessenger messenger,
        IDialogService dialogService)
        : base(navigationService, messenger, dialogService)
    {
        _recebimentoService = recebimentoService ?? throw new ArgumentNullException(nameof(recebimentoService));
        
        RegistrarRecebimentoParcialCommand = new RelayCommand<int?>(ExecuteRegistrarParcial);
        RegistrarRecebimentoCompletoCommand = new AsyncRelayCommand<int?>(ExecuteRegistrarCompletoAsync);
    }
    
    protected override async Task<IEnumerable<RecebimentoDto>> LoadDataAsync()
    {
        return MostrarApenasPendentes
            ? await _recebimentoService.ListarPendentesAsync()
            : await _recebimentoService.ListarTodosAsync();
    }
    
    protected override void NavigateToForm(int? id)
    {
        if (id.HasValue)
            NavigationService.NavigateTo<RecebimentoFormViewModel>(id.Value);
        else
            NavigationService.NavigateTo<RecebimentoFormViewModel>();
    }
    
    protected override async Task DeleteAsync(int id)
    {
        await _recebimentoService.ExcluirAsync(id);
    }
    
    protected override bool MatchFilter(RecebimentoDto item, string filter)
    {
        return item.Descricao.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }
    
    protected override bool ShouldReloadOnMessage(object message)
    {
        return message is SuccessMessage;
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
    
    private async void ExecuteRegistrarParcial(int? id)
    {
        if (!id.HasValue)
            return;
        
        try
        {
            var recebimento = Items.FirstOrDefault(r => r.Id == id.Value);
            if (recebimento == null)
                return;
            
            var valorPendente = recebimento.ValorPendente;
            
            // Utiliza metade do valor pendente como valor padrão para recebimento
            // TODO: Implementar diálogo de entrada para valor customizado
            var valorReceber = valorPendente / 2;
            
            if (valorReceber > 0)
            {
                await _recebimentoService.RegistrarRecebimentoParcialAsync(id.Value, valorReceber);
                Messenger.Send(new SuccessMessage($"Recebimento parcial de {valorReceber:C} registrado com sucesso"));
                await RecarregarAsync();
            }
        }
        catch (Exception ex)
        {
            DialogService.ShowError($"Erro ao registrar recebimento parcial: {ex.Message}", "Erro");
            Messenger.Send(new ErrorMessage("Erro ao registrar recebimento parcial", ex));
        }
    }
    
    private async Task ExecuteRegistrarCompletoAsync(int? id)
    {
        if (!id.HasValue)
            return;
        
        try
        {
            await _recebimentoService.RegistrarRecebimentoCompletoAsync(id.Value, DateTime.Now);
            Messenger.Send(new SuccessMessage("Recebimento registrado com sucesso"));
            await RecarregarAsync();
        }
        catch (Exception ex)
        {
            DialogService.ShowError($"Erro ao registrar recebimento: {ex.Message}", "Erro");
            Messenger.Send(new ErrorMessage("Erro ao registrar recebimento", ex));
        }
    }
}
