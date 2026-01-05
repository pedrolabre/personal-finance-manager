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

namespace PersonalFinanceManager.ViewModels.Acordos
{
    public class AcordosListViewModel : BaseListViewModel<AcordoDto>
    {
        private readonly IAcordoService _acordoService;
        private bool _mostrarInativos;

        protected override string EntityName => "Acordo";
        protected override string EntityNamePlural => "Acordos";

        // Alias para compatibilidade com View
        public System.Collections.ObjectModel.ObservableCollection<AcordoDto> Acordos => ItemsFiltrados;
        public ICommand NovoAcordoCommand => NovoCommand;

        public bool MostrarInativos
        {
            get => _mostrarInativos;
            set
            {
                if (SetProperty(ref _mostrarInativos, value))
                {
                    _ = RecarregarAsync();
                }
            }
        }

        public ICommand VisualizarCommand { get; }

        public AcordosListViewModel(
            IAcordoService acordoService,
            INavigationService navigationService,
            IMessenger messenger,
            IDialogService dialogService)
            : base(navigationService, messenger, dialogService)
        {
            _acordoService = acordoService ?? throw new ArgumentNullException(nameof(acordoService));
            
            VisualizarCommand = new RelayCommand<int?>(ExecuteVisualizarDetalhes);
        }

        protected override async Task<IEnumerable<AcordoDto>> LoadDataAsync()
        {
            var acordos = await _acordoService.ListarTodosAsync();
            
            if (!MostrarInativos)
            {
                acordos = acordos.Where(a => a.Ativo);
            }
            
            return acordos;
        }

        protected override void NavigateToForm(int? id)
        {
            if (id.HasValue)
                NavigationService.NavigateTo<AcordoFormViewModel>(id.Value);
            else
                NavigationService.NavigateTo<AcordoFormViewModel>();
        }

        protected override async Task DeleteAsync(int id)
        {
            await _acordoService.ExcluirAsync(id);
        }

        protected override bool MatchFilter(AcordoDto item, string filter)
        {
            return item.NomePendencia.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                   (item.Observacoes?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false);
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

        private void ExecuteVisualizarDetalhes(int? acordoId)
        {
            System.Diagnostics.Debug.WriteLine($"AcordosListViewModel - ExecuteVisualizarDetalhes chamado com ID: {acordoId}");
            if (acordoId.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"AcordosListViewModel - Navegando para AcordoDetalhesViewModel com ID: {acordoId.Value}");
                NavigationService.NavigateTo<AcordoDetalhesViewModel>(acordoId.Value);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("AcordosListViewModel - ID é null, não navegando");
            }
        }
    }
}
