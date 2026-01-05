#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalFinanceManager.Core.Dialogs;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;
using PersonalFinanceManager.Core.Navigation;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager.ViewModels.Cartoes
{
    public class CartaoFormViewModel : BaseFormViewModel<CartaoCreditoDto>
    {
        private readonly ICartaoCreditoService _cartaoService;
        
        private int? _id;
        private string _nome = string.Empty;
        private string? _banco;
        private int _diaVencimento = 10;
        private int _diaFechamento = 1;
        private decimal? _limite;
        private bool _ativo = true;
        
        protected override string EntityName => "Cartão";
        
        public string Nome
        {
            get => _nome;
            set
            {
                if (SetProperty(ref _nome, value))
                {
                    NotifySalvarCanExecuteChanged();
                }
            }
        }
        
        public string? Banco
        {
            get => _banco;
            set => SetProperty(ref _banco, value);
        }
        
        public int DiaVencimento
        {
            get => _diaVencimento;
            set
            {
                if (SetProperty(ref _diaVencimento, value))
                {
                    NotifySalvarCanExecuteChanged();
                }
            }
        }
        
        public int DiaFechamento
        {
            get => _diaFechamento;
            set
            {
                if (SetProperty(ref _diaFechamento, value))
                {
                    NotifySalvarCanExecuteChanged();
                }
            }
        }
        
        public decimal? Limite
        {
            get => _limite;
            set => SetProperty(ref _limite, value);
        }
        
        public bool Ativo
        {
            get => _ativo;
            set => SetProperty(ref _ativo, value);
        }
        
        public CartaoFormViewModel(
            ICartaoCreditoService cartaoService,
            INavigationService navigationService,
            IMessenger messenger,
            IDialogService dialogService)
            : base(navigationService, messenger, dialogService)
        {
            _cartaoService = cartaoService ?? throw new ArgumentNullException(nameof(cartaoService));
        }
        
        public override void OnNavigatedTo(object? parameter = null)
        {
            base.OnNavigatedTo(parameter);
            
            if (parameter is int id)
            {
                _id = id;
                IsEditing = true;
                _ = CarregarCartaoAsync(id);
            }
            else
            {
                IsEditing = false;
            }
        }
        
        private async Task CarregarCartaoAsync(int id)
        {
            var cartao = await _cartaoService.ObterPorIdAsync(id);
            
            if (cartao != null)
            {
                Nome = cartao.Nome;
                Banco = cartao.Banco;
                DiaVencimento = cartao.DiaVencimento;
                DiaFechamento = cartao.DiaFechamento;
                Limite = cartao.Limite;
                Ativo = cartao.Ativo;
            }
        }
        
        protected override async Task<List<string>> ValidateAsync()
        {
            var errors = new List<string>();
            
            if (string.IsNullOrWhiteSpace(Nome))
                errors.Add("Nome é obrigatório");
            
            if (DiaVencimento < 1 || DiaVencimento > 31)
                errors.Add("Dia de vencimento deve estar entre 1 e 31");
            
            if (DiaFechamento < 1 || DiaFechamento > 31)
                errors.Add("Dia de fechamento deve estar entre 1 e 31");
            
            // Validar se o nome é único
            if (!await _cartaoService.ValidarNomeUnicoAsync(Nome, _id))
                errors.Add($"Já existe um cartão com o nome '{Nome}'");
            
            return errors;
        }
        
        protected override async Task<CartaoCreditoDto> BuildDtoAsync()
        {
            return await Task.FromResult(new CartaoCreditoDto
            {
                Id = _id ?? 0,
                Nome = Nome,
                Banco = Banco,
                DiaVencimento = DiaVencimento,
                DiaFechamento = DiaFechamento,
                Limite = Limite,
                Ativo = Ativo
            });
        }
        
        protected override async Task SaveAsync(CartaoCreditoDto dto)
        {
            if (IsEditing && _id.HasValue)
            {
                await _cartaoService.AtualizarAsync(_id.Value, dto);
            }
            else
            {
                var resultado = await _cartaoService.CriarAsync(dto);
                _id = resultado.Id;
            }
        }
        
        protected override void SendSuccessMessage()
        {
            // Usar mensagens genéricas já que não há mensagens específicas para Cartão
            Messenger.Send(new SuccessMessage(IsEditing 
                ? "Cartão atualizado com sucesso" 
                : "Cartão criado com sucesso"));
        }
        
        protected override bool CanSalvar()
        {
            return !string.IsNullOrWhiteSpace(Nome) 
                && DiaVencimento >= 1 && DiaVencimento <= 31
                && DiaFechamento >= 1 && DiaFechamento <= 31
                && base.CanSalvar();
        }
    }
}
