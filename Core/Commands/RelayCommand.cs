#nullable enable
using System;
using System.Windows.Input;

namespace PersonalFinanceManager.Core.Commands;

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;
    
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
    
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }
    
    public void Execute(object? parameter)
    {
        _execute();
    }
    
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool>? _canExecute;
    
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
    
    public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object? parameter)
    {
        var value = ConvertParameter(parameter);
        return _canExecute?.Invoke(value) ?? true;
    }
    
    public void Execute(object? parameter)
    {
        var value = ConvertParameter(parameter);
        _execute(value);
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