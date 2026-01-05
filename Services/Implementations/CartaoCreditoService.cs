using PersonalFinanceManager.Data.Entities;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Models.Enums;
using PersonalFinanceManager.Repositories.Interfaces;
using PersonalFinanceManager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PersonalFinanceManager.Services.Implementations;

public class CartaoCreditoService : ICartaoCreditoService
{
    private readonly ICartaoCreditoRepository _repository;
    
    public CartaoCreditoService(ICartaoCreditoRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<CartaoCreditoDto>> ListarTodosAsync()
    {
        var cartoes = await _repository.GetAllAsync();
        return cartoes.Select(MapearParaDto);
    }
    
    public async Task<IEnumerable<CartaoCreditoDto>> ListarAtivosAsync()
    {
        var cartoes = await _repository.GetAtivosAsync();
        return cartoes.Select(MapearParaDto);
    }
    
    public async Task<CartaoCreditoDto> ObterPorIdAsync(int id)
    {
        var cartao = await _repository.GetByIdAsync(id);
        return cartao != null ? MapearParaDto(cartao) : null;
    }
    
    public async Task<CartaoCreditoDto> CriarAsync(CartaoCreditoDto dto)
    {
        await ValidarCartaoAsync(dto);
        
        var cartao = new CartaoCredito
        {
            Nome = dto.Nome,
            Banco = dto.Banco,
            DiaVencimento = dto.DiaVencimento,
            DiaFechamento = dto.DiaFechamento,
            Limite = dto.Limite ?? 0,
            Ativo = dto.Ativo
        };
        
        var resultado = await _repository.AddAsync(cartao);
        return MapearParaDto(resultado);
    }
    
    public async Task AtualizarAsync(int id, CartaoCreditoDto dto)
    {
        var cartao = await _repository.GetByIdAsync(id);
        if (cartao == null)
            throw new InvalidOperationException("Cartão não encontrado");
        
        await ValidarCartaoAsync(dto, id);
        
        cartao.Nome = dto.Nome;
        cartao.Banco = dto.Banco;
        cartao.DiaVencimento = dto.DiaVencimento;
        cartao.DiaFechamento = dto.DiaFechamento;
        cartao.Limite = dto.Limite ?? 0;
        cartao.Ativo = dto.Ativo;
        
        await _repository.UpdateAsync(cartao);
    }
    
    public async Task ExcluirAsync(int id)
    {
        var cartao = await _repository.GetByIdAsync(id);
        if (cartao == null)
            throw new InvalidOperationException("Cartão não encontrado");
        
        if (cartao.Pendencias.Any())
            throw new InvalidOperationException("Não é possível excluir cartão com pendências vinculadas");
        
        await _repository.DeleteAsync(id);
    }
    
    public async Task<bool> ValidarNomeUnicoAsync(string nome, int? ignorarId = null)
    {
        return !await _repository.ExisteCartaoComNomeAsync(nome, ignorarId ?? 0);
    }
    
    private async Task ValidarCartaoAsync(CartaoCreditoDto dto, int? ignorarId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório");
        
        if (dto.DiaVencimento < 1 || dto.DiaVencimento > 31)
            throw new ArgumentException("Dia de vencimento inválido");
        
        if (dto.DiaFechamento < 1 || dto.DiaFechamento > 31)
            throw new ArgumentException("Dia de fechamento inválido");
        
        if (await _repository.ExisteCartaoComNomeAsync(dto.Nome, ignorarId ?? 0))
            throw new ArgumentException("Já existe um cartão com este nome");
    }
    
    private CartaoCreditoDto MapearParaDto(CartaoCredito cartao)
    {
        var totalDividas = cartao.Pendencias
            .Where(p => p.Status != StatusPendencia.Quitada)
            .Sum(p => p.ValorTotal);
        
        return new CartaoCreditoDto
        {
            Id = cartao.Id,
            Nome = cartao.Nome,
            Banco = cartao.Banco,
            DiaVencimento = cartao.DiaVencimento,
            DiaFechamento = cartao.DiaFechamento,
            Limite = cartao.Limite,
            Ativo = cartao.Ativo,
            TotalDividas = totalDividas,
            QuantidadeDividas = cartao.Pendencias.Count(p => p.Status != StatusPendencia.Quitada)
        };
    }
}
