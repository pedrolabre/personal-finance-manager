#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalFinanceManager.Core.Dialogs;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;
using PersonalFinanceManager.Core.Navigation;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Models.Enums;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager.ViewModels.Recebimentos;

public class RecebimentoFormViewModel : BaseFormViewModel<RecebimentoDto>
{
    private readonly IRecebimentoService _recebimentoService;
    
    private int? _id;
    private string _descricao = string.Empty;
    private CategoriaRecebimento _categoria = CategoriaRecebimento.Salario;
    private DateTime _dataPrevista = DateTime.Now;
    private DateTime? _dataRecebimento;
    private decimal _valorEsperado;
    private decimal _valorRecebido;
    private bool _recebimentoCompleto;
    
    protected override string EntityName => "Recebimento";
    
    public string Descricao
    {
        get => _descricao;
        set
        {
            if (SetProperty(ref _descricao, value))
            {
                NotifySalvarCanExecuteChanged();
            }
        }
    }
    
    public CategoriaRecebimento Categoria
    {
        get => _categoria;
        set => SetProperty(ref _categoria, value);
    }
    
    public DateTime DataPrevista
    {
        get => _dataPrevista;
        set => SetProperty(ref _dataPrevista, value);
    }
    
    public DateTime? DataRecebimento
    {
        get => _dataRecebimento;
        set => SetProperty(ref _dataRecebimento, value);
    }
    
    public decimal ValorEsperado
    {
        get => _valorEsperado;
        set
        {
            if (SetProperty(ref _valorEsperado, value))
            {
                NotifySalvarCanExecuteChanged();
            }
        }
    }
    
    public decimal ValorRecebido
    {
        get => _valorRecebido;
        set => SetProperty(ref _valorRecebido, value);
    }
    
    public bool RecebimentoCompleto
    {
        get => _recebimentoCompleto;
        set => SetProperty(ref _recebimentoCompleto, value);
    }
    
    public Array Categorias => Enum.GetValues(typeof(CategoriaRecebimento));
    
    public RecebimentoFormViewModel(
        IRecebimentoService recebimentoService,
        INavigationService navigationService,
        IMessenger messenger,
        IDialogService dialogService)
        : base(navigationService, messenger, dialogService)
    {
        _recebimentoService = recebimentoService ?? throw new ArgumentNullException(nameof(recebimentoService));
    }
    
    public override void OnNavigatedTo(object? parameter = null)
    {
        base.OnNavigatedTo(parameter);
        
        if (parameter is int id)
        {
            _id = id;
            IsEditing = true;
            _ = CarregarRecebimentoAsync(id);
        }
        else
        {
            IsEditing = false;
        }
    }
    
    private async Task CarregarRecebimentoAsync(int id)
    {
        var recebimento = await _recebimentoService.ObterPorIdAsync(id);
        
        if (recebimento != null)
        {
            Descricao = recebimento.Descricao;
            Categoria = recebimento.Categoria;
            DataPrevista = recebimento.DataPrevista;
            DataRecebimento = recebimento.DataRecebimento;
            ValorEsperado = recebimento.ValorEsperado;
            ValorRecebido = recebimento.ValorRecebido;
            RecebimentoCompleto = recebimento.RecebimentoCompleto;
        }
    }
    
    protected override Task<List<string>> ValidateAsync()
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(Descricao))
            errors.Add("A descrição é obrigatória.");
        
        if (ValorEsperado <= 0)
            errors.Add("O valor esperado deve ser maior que zero.");
        
        return Task.FromResult(errors);
    }
    
    protected override Task<RecebimentoDto> BuildDtoAsync()
    {
        var dto = new RecebimentoDto
        {
            Id = _id ?? 0,
            Descricao = Descricao,
            Categoria = Categoria,
            DataPrevista = DataPrevista,
            DataRecebimento = DataRecebimento,
            ValorEsperado = ValorEsperado,
            ValorRecebido = ValorRecebido,
            RecebimentoCompleto = RecebimentoCompleto
        };
        
        return Task.FromResult(dto);
    }
    
    protected override async Task SaveAsync(RecebimentoDto dto)
    {
        if (_id.HasValue)
        {
            await _recebimentoService.AtualizarAsync(_id.Value, dto);
        }
        else
        {
            await _recebimentoService.CriarAsync(dto);
        }
    }
    
    protected override void SendSuccessMessage()
    {
        Messenger.Send(new SuccessMessage($"Recebimento {(IsEditing ? "atualizado" : "criado")} com sucesso"));
    }
    
    protected override bool CanSalvar()
    {
        return base.CanSalvar() && 
               !string.IsNullOrWhiteSpace(Descricao) && 
               ValorEsperado > 0;
    }
}
