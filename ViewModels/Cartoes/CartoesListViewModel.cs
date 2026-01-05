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

namespace PersonalFinanceManager.ViewModels.Cartoes
{
    public class CartoesListViewModel : BaseListViewModel<CartaoCreditoDto>
    {
        private readonly ICartaoCreditoService _cartaoService;
        private bool _mostrarInativos;

        protected override string EntityName => "Cartão de Crédito";
        protected override string EntityNamePlural => "Cartões de Crédito";
        // Alias para compatibilidade com View
        public System.Collections.ObjectModel.ObservableCollection<CartaoCreditoDto> Cartoes => ItemsFiltrados;
        public ICommand NovoCartaoCommand => NovoCommand;
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

        public ICommand VisualizarPendenciasCommand { get; }

        public CartoesListViewModel(
            ICartaoCreditoService cartaoService,
            INavigationService navigationService,
            IMessenger messenger,
            IDialogService dialogService)
            : base(navigationService, messenger, dialogService)
        {
            _cartaoService = cartaoService ?? throw new ArgumentNullException(nameof(cartaoService));
            
            VisualizarPendenciasCommand = new RelayCommand<int?>(ExecuteVisualizarPendencias);
        }

        protected override async Task<IEnumerable<CartaoCreditoDto>> LoadDataAsync()
        {
            return MostrarInativos 
                ? await _cartaoService.ListarTodosAsync()
                : await _cartaoService.ListarAtivosAsync();
        }

        protected override void NavigateToForm(int? id)
        {
            if (id.HasValue)
                NavigationService.NavigateTo<CartaoFormViewModel>(id.Value);
            else
                NavigationService.NavigateTo<CartaoFormViewModel>();
        }

        protected override async Task DeleteAsync(int id)
        {
            await _cartaoService.ExcluirAsync(id);
        }

        protected override bool MatchFilter(CartaoCreditoDto item, string filter)
        {
            return item.Nome.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                   (item.Banco?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false);
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

        private void ExecuteVisualizarPendencias(int? cartaoId)
        {
            if (cartaoId.HasValue)
            {
                // Navegar para Pendências e aplicar filtro por cartão
                NavigationService.NavigateTo<Pendencias.PendenciasListViewModel>();
                // TODO: Implementar filtro por cartão no futuro
                Messenger.Send(new InfoMessage($"Lista de pendências carregada. Filtro por cartão será implementado em breve."));
            }
        }
    }
}
