#nullable enable
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PersonalFinanceManager.Core.Commands;

public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;
    
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
    
    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke() ?? true);
    }
    
    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;
        
        _isExecuting = true;
        RaiseCanExecuteChanged();
        
        try
        {
            await _execute();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao executar comando: {ex.Message}\n\n{ex.StackTrace}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }
    
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}

public class AsyncRelayCommand<T> : ICommand
{
    private readonly Func<T, Task> _execute;
    private readonly Func<T, bool>? _canExecute;
    private bool _isExecuting;
    
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
    
    public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object? parameter)
    {
        if (_isExecuting)
            return false;
            
        var value = ConvertParameter(parameter);
        return _canExecute?.Invoke(value) ?? true;
    }
    
    public async void Execute(object? parameter)
    {
        var value = ConvertParameter(parameter);
        
        if (!CanExecute(parameter))
            return;
        
        _isExecuting = true;
        RaiseCanExecuteChanged();
        
        try
        {
            await _execute(value);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao executar comando: {ex.Message}\n\n{ex.StackTrace}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }
    
    private T ConvertParameter(object? parameter)
    {
        // Handle null
        if (parameter == null)
            return default!;
            
        // Handle direct type match
        if (parameter is T typedValue)
            return typedValue;
            
        // Handle nullable types (e.g., int? when parameter is int)
        var targetType = typeof(T);
        var underlyingType = Nullable.GetUnderlyingType(targetType);
        
        if (underlyingType != null)
        {
            // T is nullable, convert the value
            return (T)Convert.ChangeType(parameter, underlyingType);
        }
        
        // Try direct conversion
        return (T)Convert.ChangeType(parameter, targetType);
    }
    
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}