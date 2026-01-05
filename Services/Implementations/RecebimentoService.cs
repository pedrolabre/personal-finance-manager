using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalFinanceManager.Data.Entities;
using System.Linq;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Repositories.Interfaces;
using PersonalFinanceManager.Services.Interfaces;

namespace PersonalFinanceManager.Services.Implementations;

public class RecebimentoService : IRecebimentoService
{
    private readonly IRecebimentoRepository _repository;
    
    public RecebimentoService(IRecebimentoRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<RecebimentoDto>> ListarTodosAsync()
    {
        var recebimentos = await _repository.GetAllAsync();
        return recebimentos.Select(MapearParaDto);
    }
    
    public async Task<IEnumerable<RecebimentoDto>> ListarPendentesAsync()
    {
        var recebimentos = await _repository.GetPendentesAsync();
        return recebimentos.Select(MapearParaDto);
    }
    
    public async Task<IEnumerable<RecebimentoDto>> ListarAtrasadosAsync()
    {
        var recebimentos = await _repository.GetAtrasadosAsync();
        return recebimentos.Select(MapearParaDto);
    }
    
    public async Task<RecebimentoDto> ObterPorIdAsync(int id)
    {
        var recebimento = await _repository.GetByIdAsync(id);
        return recebimento != null ? MapearParaDto(recebimento) : null;
    }
    
    public async Task<RecebimentoDto> CriarAsync(RecebimentoDto dto)
    {
        ValidarRecebimento(dto);
        
        var recebimento = new Recebimento
        {
            Descricao = dto.Descricao,
            Categoria = dto.Categoria,
            DataPrevista = dto.DataPrevista,
            DataRecebimento = dto.DataRecebimento,
            ValorEsperado = dto.ValorEsperado,
            ValorRecebido = dto.ValorRecebido,
            RecebimentoCompleto = dto.RecebimentoCompleto
        };
        
        var resultado = await _repository.AddAsync(recebimento);
        return MapearParaDto(resultado);
    }
    
    public async Task AtualizarAsync(int id, RecebimentoDto dto)
    {
        var recebimento = await _repository.GetByIdAsync(id);
        if (recebimento == null)
            throw new InvalidOperationException("Recebimento não encontrado");
        
        ValidarRecebimento(dto);
        
        recebimento.Descricao = dto.Descricao;
        recebimento.Categoria = dto.Categoria;
        recebimento.DataPrevista = dto.DataPrevista;
        recebimento.DataRecebimento = dto.DataRecebimento;
        recebimento.ValorEsperado = dto.ValorEsperado;
        recebimento.ValorRecebido = dto.ValorRecebido;
        recebimento.RecebimentoCompleto = dto.RecebimentoCompleto;
        
        await _repository.UpdateAsync(recebimento);
    }
    
    public async Task ExcluirAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
    
    public async Task RegistrarRecebimentoParcialAsync(int id, decimal valorRecebido)
    {
        var recebimento = await _repository.GetByIdAsync(id);
        if (recebimento == null)
            throw new InvalidOperationException("Recebimento não encontrado");
        
        if (valorRecebido <= 0)
            throw new ArgumentException("Valor deve ser maior que zero");
        
        recebimento.ValorRecebido += valorRecebido;
        
        if (recebimento.ValorRecebido >= recebimento.ValorEsperado)
        {
            recebimento.RecebimentoCompleto = true;
            recebimento.DataRecebimento = DateTime.Now;
        }
        
        await _repository.UpdateAsync(recebimento);
    }
    
    public async Task RegistrarRecebimentoCompletoAsync(int id, DateTime dataRecebimento)
    {
        var recebimento = await _repository.GetByIdAsync(id);
        if (recebimento == null)
            throw new InvalidOperationException("Recebimento não encontrado");
        
        recebimento.ValorRecebido = recebimento.ValorEsperado;
        recebimento.RecebimentoCompleto = true;
        recebimento.DataRecebimento = dataRecebimento;
        
        await _repository.UpdateAsync(recebimento);
    }
    
    private void ValidarRecebimento(RecebimentoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Descricao))
            throw new ArgumentException("Descrição é obrigatória");
        
        if (dto.ValorEsperado <= 0)
            throw new ArgumentException("Valor esperado deve ser maior que zero");
        
        if (dto.ValorRecebido < 0)
            throw new ArgumentException("Valor recebido não pode ser negativo");
        
        if (dto.ValorRecebido > dto.ValorEsperado)
            throw new ArgumentException("Valor recebido não pode ser maior que o esperado");
    }
    
    private RecebimentoDto MapearParaDto(Recebimento recebimento)
    {
        return new RecebimentoDto
        {
            Id = recebimento.Id,
            Descricao = recebimento.Descricao,
            Categoria = recebimento.Categoria,
            DataPrevista = recebimento.DataPrevista,
            DataRecebimento = recebimento.DataRecebimento,
            ValorEsperado = recebimento.ValorEsperado,
            ValorRecebido = recebimento.ValorRecebido,
            RecebimentoCompleto = recebimento.RecebimentoCompleto
        };
    }
}
