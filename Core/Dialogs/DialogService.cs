#nullable enable
using System.Threading.Tasks;
using System.Windows;

namespace PersonalFinanceManager.Core.Dialogs;

public class DialogService : IDialogService
{
    public void ShowInfo(string message, string title = "Informação")
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        });
    }

    public void ShowWarning(string message, string title = "Aviso")
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        });
    }

    public void ShowError(string message, string title = "Erro")
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        });
    }

    public void ShowSuccess(string message, string title = "Sucesso")
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        });
    }

    public bool Confirm(string message, string title = "Confirmação")
    {
        bool result = false;
        Application.Current.Dispatcher.Invoke(() =>
        {
            var dialogResult = MessageBox.Show(
                message, 
                title, 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question
            );
            result = dialogResult == MessageBoxResult.Yes;
        });
        return result;
    }

    public Task<bool> ConfirmAsync(string message, string title = "Confirmação")
    {
        return Task.Run(() => Confirm(message, title));
    }

    public string? Prompt(string message, string title = "Entrada", string defaultValue = "")
    {
        // Para WPF, precisaríamos criar uma janela customizada
        // Por enquanto, retornamos null indicando que não está implementado
        ShowWarning("Função Prompt não implementada para WPF.", "Não Implementado");
        return null;
    }
}
