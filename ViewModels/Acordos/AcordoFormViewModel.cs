#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using PersonalFinanceManager.ViewModels.Base;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.Core.Navigation;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Dialogs;
using PersonalFinanceManager.Core.Messaging.Messages;
using PersonalFinanceManager.Models.DTOs;

namespace PersonalFinanceManager.ViewModels.Acordos
{
    public class AcordoFormViewModel : BaseFormViewModel<AcordoDto>
    {
        private readonly IAcordoService _acordoService;
        
        private int? _id;
        private string _nomeAcordo = string.Empty;
        private string _descricao = string.Empty;
        private decimal _valorTotal;
        private int _numeroParcelas = 1;
        private DateTime _dataInicio = DateTime.Today;
        private bool _ativo = true;

        protected override string EntityName => "Acordo";

        public string NomeAcordo
        {
            get => _nomeAcordo;
            set
            {
                if (SetProperty(ref _nomeAcordo, value))
                {
                    NotifySalvarCanExecuteChanged();
                }
            }
        }

        public string Descricao
        {
            get => _descricao;
            set => SetProperty(ref _descricao, value);
        }

        public decimal ValorTotal
        {
            get => _valorTotal;
            set
            {
                if (SetProperty(ref _valorTotal, value))
                {
                    NotifySalvarCanExecuteChanged();
                }
            }
        }

        public int NumeroParcelas
        {
            get => _numeroParcelas;
            set
            {
                if (SetProperty(ref _numeroParcelas, value))
                {
                    NotifySalvarCanExecuteChanged();
                }
            }
        }

        public DateTime DataInicio
        {
            get => _dataInicio;
            set => SetProperty(ref _dataInicio, value);
        }

        public bool Ativo
        {
            get => _ativo;
            set => SetProperty(ref _ativo, value);
        }

        public AcordoFormViewModel(
            IAcordoService acordoService,
            INavigationService navigationService,
            IMessenger messenger,
            IDialogService dialogService)
            : base(navigationService, messenger, dialogService)
        {
            _acordoService = acordoService ?? throw new ArgumentNullException(nameof(acordoService));
        }

        public override void OnNavigatedTo(object? parameter = null)
        {
            base.OnNavigatedTo(parameter);

            if (parameter is int acordoId)
            {
                _id = acordoId;
                IsEditing = true;
                _ = CarregarAcordoAsync(acordoId);
            }
            else
            {
                IsEditing = false;
            }
        }

        private async Task CarregarAcordoAsync(int acordoId)
        {
            var acordo = await _acordoService.ObterPorIdAsync(acordoId);
            if (acordo != null)
            {
                NomeAcordo = acordo.NomePendencia;
                Descricao = acordo.Observacoes ?? string.Empty;
                ValorTotal = acordo.ValorTotal;
                NumeroParcelas = acordo.NumeroParcelas;
                DataInicio = acordo.DataAcordo;
                Ativo = acordo.Ativo;
            }
        }

        protected override Task<List<string>> ValidateAsync()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(NomeAcordo))
                errors.Add("O nome do acordo é obrigatório.");

            if (ValorTotal <= 0)
                errors.Add("O valor total deve ser maior que zero.");

            if (NumeroParcelas <= 0)
                errors.Add("O número de parcelas deve ser maior que zero.");

            return Task.FromResult(errors);
        }

        protected override Task<AcordoDto> BuildDtoAsync()
        {
            var dto = new AcordoDto
            {
                Id = _id ?? 0,
                NomePendencia = NomeAcordo,
                Observacoes = Descricao,
                ValorTotal = ValorTotal,
                NumeroParcelas = NumeroParcelas,
                DataAcordo = DataInicio,
                Ativo = Ativo
            };

            return Task.FromResult(dto);
        }

        protected override async Task SaveAsync(AcordoDto dto)
        {
            if (_id.HasValue)
            {
                await _acordoService.AtualizarAsync(_id.Value, dto);
            }
            else
            {
                await _acordoService.CriarAsync(dto);
            }
        }

        protected override void SendSuccessMessage()
        {
            Messenger.Send(new SuccessMessage($"Acordo {(IsEditing ? "atualizado" : "criado")} com sucesso"));
        }

        protected override bool CanSalvar()
        {
            return base.CanSalvar() &&
                   !string.IsNullOrWhiteSpace(NomeAcordo) &&
                   ValorTotal > 0 &&
                   NumeroParcelas > 0;
        }
    }
}
