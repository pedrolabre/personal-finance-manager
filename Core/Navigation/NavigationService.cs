
using System;
using System.Collections.Generic;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager.Core.Navigation;

public class NavigationService : INavigationService
{
    private readonly Func<Type, ViewModelBase> _viewModelFactory;
    private readonly Stack<ViewModelBase> _navigationStack;
    private ViewModelBase _currentViewModel;
    
    public event EventHandler<ViewModelBase> CurrentViewModelChanged;
    
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            if (_currentViewModel == value)
                return;
            
            _currentViewModel?.OnNavigatedFrom();
            _currentViewModel = value;
            CurrentViewModelChanged?.Invoke(this, _currentViewModel!);
        }
    }
    
    public bool CanNavigateBack => _navigationStack.Count > 0;
    
    public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
        _navigationStack = new Stack<ViewModelBase>();
    }
    
    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        NavigateTo<TViewModel>(null);
    }
    
    public void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModelBase
    {
        System.Diagnostics.Debug.WriteLine($"NavigationService - NavigateTo<{typeof(TViewModel).Name}> com parâmetro: {parameter} (tipo: {parameter?.GetType().Name ?? "null"})");
        
        if (_currentViewModel != null)
        {
            _navigationStack.Push(_currentViewModel);
        }
        
        var viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
        System.Diagnostics.Debug.WriteLine($"NavigationService - ViewModel criado: {viewModel?.GetType().Name ?? "null"}");
        CurrentViewModel = viewModel;
        CurrentViewModel?.OnNavigatedTo(parameter);
    }
    
    public void NavigateBack()
    {
        if (!CanNavigateBack)
            return;
        
        var previousViewModel = _navigationStack.Pop();
        CurrentViewModel = previousViewModel;
        CurrentViewModel?.OnNavigatedTo();
    }
    
    public void ClearHistory()
    {
        _navigationStack.Clear();
    }
}
