using System.Windows.Input;
using System;
using PersonalFinanceManager.Core.Commands;
using PersonalFinanceManager.Core.Navigation;
using PersonalFinanceManager.ViewModels.Base;
using PersonalFinanceManager.ViewModels.Pendencias;
using PersonalFinanceManager.ViewModels.Acordos;
using PersonalFinanceManager.ViewModels.Importacao;
using PersonalFinanceManager.ViewModels.Relatorios;
using PersonalFinanceManager.ViewModels.Configuracoes;

using PersonalFinanceManager.ViewModels.Cartoes;
using PersonalFinanceManager.ViewModels.Recebimentos;

namespace PersonalFinanceManager.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private ViewModelBase _currentView;
    
    public ViewModelBase CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }
    
    public ICommand NavigateToDashboardCommand { get; }
    public ICommand NavigateToPendenciasCommand { get; }
    public ICommand NavigateToCartoesCommand { get; }
    public ICommand NavigateToAcordosCommand { get; }
    public ICommand NavigateToRecebimentosCommand { get; }
    public ICommand NavigateToImportCommand { get; }
    public ICommand NavigateToReportsCommand { get; }
    public ICommand NavigateToSettingsCommand { get; }
    
    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
        NavigateToDashboardCommand = new RelayCommand(ExecuteNavigateToDashboard);
        NavigateToPendenciasCommand = new RelayCommand(ExecuteNavigateToPendencias);
        NavigateToCartoesCommand = new RelayCommand(ExecuteNavigateToCartoes);
        NavigateToAcordosCommand = new RelayCommand(ExecuteNavigateToAcordos);
        NavigateToRecebimentosCommand = new RelayCommand(ExecuteNavigateToRecebimentos);
        NavigateToImportCommand = new RelayCommand(ExecuteNavigateToImport);
        NavigateToReportsCommand = new RelayCommand(ExecuteNavigateToReports);
        NavigateToSettingsCommand = new RelayCommand(ExecuteNavigateToSettings);
        // Navegação inicial correta via NavigationService
        _navigationService.NavigateTo<DashboardViewModel>();
    }
    
    private void OnCurrentViewModelChanged(object sender, ViewModelBase viewModel)
    {
        CurrentView = viewModel;
    }
    
    private void ExecuteNavigateToDashboard()
    {
        _navigationService.NavigateTo<DashboardViewModel>();
    }
    
    private void ExecuteNavigateToPendencias()
    {
        _navigationService.NavigateTo<PendenciasListViewModel>();
    }
    
    private void ExecuteNavigateToCartoes()
    {
        _navigationService.NavigateTo<CartoesListViewModel>();
    }
    
    private void ExecuteNavigateToAcordos()
    {
        _navigationService.NavigateTo<AcordosListViewModel>();
    }
    
    private void ExecuteNavigateToRecebimentos()
    {
        _navigationService.NavigateTo<RecebimentosListViewModel>();
    }
    
    private void ExecuteNavigateToImport()
    {
        _navigationService.NavigateTo<ImportacaoViewModel>();
    }
    
    private void ExecuteNavigateToReports()
    {
        _navigationService.NavigateTo<RelatoriosViewModel>();
    }
    
    private void ExecuteNavigateToSettings()
    {
        _navigationService.NavigateTo<ConfiguracoesViewModel>();
    }
}
