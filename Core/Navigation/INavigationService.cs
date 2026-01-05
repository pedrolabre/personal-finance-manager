#nullable enable
using System;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager.Core.Navigation;

public interface INavigationService
{
    ViewModelBase? CurrentViewModel { get; }
    bool CanNavigateBack { get; }
    
    event EventHandler<ViewModelBase>? CurrentViewModelChanged;
    
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModelBase;
    void NavigateBack();
    void ClearHistory();
}
