#nullable enable
using System.Threading.Tasks;

namespace PersonalFinanceManager.Core.Dialogs;

public interface IDialogService
{
    /// <summary>
    /// Exibe uma mensagem informativa ao usuário
    /// </summary>
    void ShowInfo(string message, string title = "Informação");
    
    /// <summary>
    /// Exibe uma mensagem de aviso ao usuário
    /// </summary>
    void ShowWarning(string message, string title = "Aviso");
    
    /// <summary>
    /// Exibe uma mensagem de erro ao usuário
    /// </summary>
    void ShowError(string message, string title = "Erro");
    
    /// <summary>
    /// Exibe uma mensagem de sucesso ao usuário
    /// </summary>
    void ShowSuccess(string message, string title = "Sucesso");
    
    /// <summary>
    /// Solicita confirmação do usuário para uma ação
    /// </summary>
    /// <returns>True se o usuário confirmar, False caso contrário</returns>
    bool Confirm(string message, string title = "Confirmação");
    
    /// <summary>
    /// Solicita confirmação assíncrona do usuário para uma ação
    /// </summary>
    Task<bool> ConfirmAsync(string message, string title = "Confirmação");
    
    /// <summary>
    /// Solicita uma entrada de texto do usuário
    /// </summary>
    string? Prompt(string message, string title = "Entrada", string defaultValue = "");
}
