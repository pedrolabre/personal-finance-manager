#nullable enable
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PersonalFinanceManager.Core.Commands;
using PersonalFinanceManager.Core.Dialogs;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;
using PersonalFinanceManager.Core.Navigation;

namespace PersonalFinanceManager.ViewModels.Base;

/// <summary>
/// Classe base para ViewModels de formulários (Create/Edit)
/// Implementa Template Method Pattern para fluxo comum de salvamento
/// </summary>
public abstract class BaseFormViewModel<TDto> : ViewModelBase where TDto : class
{
    protected readonly INavigationService NavigationService;
    protected readonly IMessenger Messenger;
    protected readonly IDialogService DialogService;
    
    private bool _isSaving;
    private bool _isEditing;
    
    public bool IsSaving
    {
        get => _isSaving;
        set => SetProperty(ref _isSaving, value);
    }
    
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }
    
    public virtual string Titulo => IsEditing ? $"Editar {EntityName}" : $"Novo {EntityName}";
    
    protected abstract string EntityName { get; }
    
    public ICommand SalvarCommand { get; }
    public ICommand CancelarCommand { get; }
    
    private AsyncRelayCommand? _salvarCommandImpl;
    
    protected BaseFormViewModel(
        INavigationService navigationService,
        IMessenger messenger,
        IDialogService dialogService)
    {
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        Messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        
        _salvarCommandImpl = new AsyncRelayCommand(ExecuteSalvarAsync, CanSalvar);
        SalvarCommand = _salvarCommandImpl;
        CancelarCommand = new RelayCommand(ExecuteCancelar);
    }
    
    /// <summary>
    /// Dispara notificação para o comando SalvarCommand reavaliar CanExecute
    /// Deve ser chamado quando propriedades que afetam CanSalvar() mudarem
    /// </summary>
    protected void NotifySalvarCanExecuteChanged()
    {
        _salvarCommandImpl?.RaiseCanExecuteChanged();
    }
    
    /// <summary>
    /// Template Method: Define o fluxo de salvamento
    /// </summary>
    private async Task ExecuteSalvarAsync()
    {
        if (IsSaving) return;
        
        try
        {
            IsSaving = true;
            
            // 1. Validar
            var validationErrors = await ValidateAsync();
            if (validationErrors.Count > 0)
            {
                DialogService.ShowWarning(
                    string.Join("\n", validationErrors),
                    "Validação"
                );
                return;
            }
            
            // 2. Construir DTO
            var dto = await BuildDtoAsync();
            
            // 3. Salvar
            await SaveAsync(dto);
            
            // 4. Notificar sucesso
            DialogService.ShowSuccess(
                $"{EntityName} {(IsEditing ? "atualizado" : "criado")} com sucesso!",
                "Sucesso"
            );
            
            // 5. Enviar mensagem
            SendSuccessMessage();
            
            // 6. Navegar de volta
            NavigationService.NavigateBack();
        }
        catch (Exception ex)
        {
            // Extrair a mensagem de erro mais detalhada (incluindo inner exceptions)
            var errorMessage = ex.Message;
            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                errorMessage += $"\n\nDetalhes: {innerEx.Message}";
                innerEx = innerEx.InnerException;
            }
            
            DialogService.ShowError(
                $"Erro ao salvar {EntityName.ToLower()}:\n\n{errorMessage}",
                "Erro"
            );
            Messenger.Send(new ErrorMessage($"Erro ao salvar {EntityName.ToLower()}", ex));
        }
        finally
        {
            IsSaving = false;
        }
    }
    
    private void ExecuteCancelar()
    {
        if (HasUnsavedChanges())
        {
            var confirmar = DialogService.Confirm(
                "Existem alterações não salvas. Deseja realmente cancelar?",
                "Confirmar Cancelamento"
            );
            
            if (!confirmar) return;
        }
        
        NavigationService.NavigateBack();
    }
    
    protected virtual bool CanSalvar()
    {
        return !IsSaving;
    }
    
    /// <summary>
    /// Valida os dados do formulário
    /// </summary>
    /// <returns>Lista de erros de validação (vazia se válido)</returns>
    protected abstract Task<System.Collections.Generic.List<string>> ValidateAsync();
    
    /// <summary>
    /// Constrói o DTO a partir dos dados do formulário
    /// </summary>
    protected abstract Task<TDto> BuildDtoAsync();
    
    /// <summary>
    /// Salva o DTO (create ou update)
    /// </summary>
    protected abstract Task SaveAsync(TDto dto);
    
    /// <summary>
    /// Envia mensagem de sucesso apropriada (Created/Updated)
    /// </summary>
    protected abstract void SendSuccessMessage();
    
    /// <summary>
    /// Verifica se há alterações não salvas
    /// </summary>
    protected virtual bool HasUnsavedChanges()
    {
        // Implementações concretas podem sobrescrever para verificar dirty flag
        return false;
    }
}
