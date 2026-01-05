#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceManager.Core.Dialogs;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;
using PersonalFinanceManager.Core.Navigation;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Models.Enums;
using PersonalFinanceManager.Services.Interfaces;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager.ViewModels.Pendencias;

public class PendenciaFormViewModel : BaseFormViewModel<PendenciaDto>
{
    private readonly IPendenciaService _pendenciaService;
    private readonly ICartaoCreditoService _cartaoService;
    
    private int? _id;
    private string _nome = string.Empty;
    private string? _descricao;
    private decimal _valorTotal;
    private Prioridade _prioridade = Prioridade.Media;
    private StatusPendencia _status = StatusPendencia.EmAberto;
    private TipoDivida _tipoDivida = TipoDivida.Outros;
    private CartaoCreditoDto? _cartaoSelecionado;
    private bool _parcelada;
    private int _quantidadeParcelas = 1;
    private int _intervaloDiasParcelas = 30;
    private DateTime _dataVencimento = DateTime.Today.AddDays(30);
    
    protected override string EntityName => "Pendência";
    
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
    
    public string? Descricao
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
    
    public Prioridade Prioridade
    {
        get => _prioridade;
        set => SetProperty(ref _prioridade, value);
    }
    
    public StatusPendencia Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }
    
    public TipoDivida TipoDivida
    {
        get => _tipoDivida;
        set
        {
            if (SetProperty(ref _tipoDivida, value))
            {
                OnPropertyChanged(nameof(MostrarCartao));
            }
        }
    }
    
    public CartaoCreditoDto? CartaoSelecionado
    {
        get => _cartaoSelecionado;
        set => SetProperty(ref _cartaoSelecionado, value);
    }
    
    public bool Parcelada
    {
        get => _parcelada;
        set
        {
            if (SetProperty(ref _parcelada, value))
            {
                OnPropertyChanged(nameof(MostrarCamposParcelas));                OnPropertyChanged(nameof(MostrarCamposParcelas));            }
        }
    }
    
    public int QuantidadeParcelas
    {
        get => _quantidadeParcelas;
        set => SetProperty(ref _quantidadeParcelas, value);
    }
    
    public int IntervaloDiasParcelas
    {
        get => _intervaloDiasParcelas;
        set => SetProperty(ref _intervaloDiasParcelas, value);
    }
    
    public DateTime DataVencimento
    {
        get => _dataVencimento;
        set => SetProperty(ref _dataVencimento, value);
    }
    
    public bool MostrarCartao => TipoDivida == TipoDivida.CartaoCredito;
    public bool MostrarCamposParcelas => Parcelada;
    
    public ObservableCollection<CartaoCreditoDto> Cartoes { get; }
    public Array Prioridades => Enum.GetValues(typeof(Prioridade));
    public Array StatusList => Enum.GetValues(typeof(StatusPendencia));
    public Array TiposDivida => Enum.GetValues(typeof(TipoDivida));
    
    public PendenciaFormViewModel(
        IPendenciaService pendenciaService,
        ICartaoCreditoService cartaoService,
        INavigationService navigationService,
        IMessenger messenger,
        IDialogService dialogService)
        : base(navigationService, messenger, dialogService)
    {
        _pendenciaService = pendenciaService ?? throw new ArgumentNullException(nameof(pendenciaService));
        _cartaoService = cartaoService ?? throw new ArgumentNullException(nameof(cartaoService));
        
        Cartoes = new ObservableCollection<CartaoCreditoDto>();
    }
    
    public override void OnNavigatedTo(object? parameter = null)
    {
        base.OnNavigatedTo(parameter);
        _ = CarregarCartoesAsync();
        
        if (parameter is int id)
        {
            _id = id;
            IsEditing = true;
            _ = CarregarPendenciaAsync(id);
        }
        else
        {
            IsEditing = false;
        }
    }
    
    private async Task CarregarCartoesAsync()
    {
        var cartoes = await _cartaoService.ListarAtivosAsync();
        
        Cartoes.Clear();
        foreach (var cartao in cartoes)
        {
            Cartoes.Add(cartao);
        }
    }
    
    private async Task CarregarPendenciaAsync(int id)
    {
        var pendencia = await _pendenciaService.ObterPorIdAsync(id);
        
        if (pendencia != null)
        {
            Nome = pendencia.Nome;
            Descricao = pendencia.Descricao;
            ValorTotal = pendencia.ValorTotal;
            Prioridade = pendencia.Prioridade;
            Status = pendencia.Status;
            TipoDivida = pendencia.TipoDivida;
            DataVencimento = pendencia.DataVencimento ?? DateTime.Today;
            Parcelada = pendencia.Parcelada;
            QuantidadeParcelas = pendencia.QuantidadeParcelas;
            IntervaloDiasParcelas = pendencia.IntervaloDiasParcelas;
            
            if (pendencia.CartaoCreditoId.HasValue)
            {
                CartaoSelecionado = Cartoes.FirstOrDefault(c => c.Id == pendencia.CartaoCreditoId.Value);
            }
        }
    }
    
    protected override async Task<List<string>> ValidateAsync()
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(Nome))
            errors.Add("Nome é obrigatório");
        
        if (ValorTotal <= 0)
            errors.Add("Valor total deve ser maior que zero");
        
        if (TipoDivida == TipoDivida.CartaoCredito && CartaoSelecionado == null)
            errors.Add("Selecione um cartão de crédito");
        
        if (Parcelada)
        {
            if (QuantidadeParcelas <= 0)
                errors.Add("Quantidade de parcelas deve ser maior que zero");
            
            if (IntervaloDiasParcelas <= 0)
                errors.Add("Intervalo entre parcelas deve ser maior que zero");
        }
        
        return await Task.FromResult(errors);
    }
    
    protected override async Task<PendenciaDto> BuildDtoAsync()
    {
        return await Task.FromResult(new PendenciaDto
        {
            Id = _id ?? 0,
            Nome = Nome,
            Descricao = Descricao,
            ValorTotal = ValorTotal,
            Prioridade = Prioridade,
            Status = Status,
            TipoDivida = TipoDivida,
            CartaoCreditoId = CartaoSelecionado?.Id,
            Parcelada = Parcelada,
            QuantidadeParcelas = Parcelada ? QuantidadeParcelas : 0,
            IntervaloDiasParcelas = Parcelada ? IntervaloDiasParcelas : 30,
            DataVencimento = DataVencimento
        });
    }
    
    protected override async Task SaveAsync(PendenciaDto dto)
    {
        if (IsEditing && _id.HasValue)
        {
            await _pendenciaService.AtualizarAsync(_id.Value, dto);
        }
        else
        {
            var resultado = await _pendenciaService.CriarAsync(dto);
            _id = resultado.Id; // Armazenar o ID retornado para a mensagem
        }
    }
    
    protected override void SendSuccessMessage()
    {
        if (IsEditing)
        {
            Messenger.Send(new PendenciaAtualizadaMessage(_id ?? 0));
        }
        else
        {
            Messenger.Send(new PendenciaCriadaMessage(_id ?? 0));
        }
    }
    
    protected override bool CanSalvar()
    {
        return !string.IsNullOrWhiteSpace(Nome) && ValorTotal > 0 && base.CanSalvar();
    }
}
