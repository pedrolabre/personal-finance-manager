using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Data.Entities;
using PersonalFinanceManager.Models.Enums;
using PersonalFinanceManager.Repositories.Interfaces;

namespace PersonalFinanceManager.Repositories.Implementations;

public class PendenciaRepository : IPendenciaRepository
{
    private readonly AppDbContext _context;
    
    public PendenciaRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<IEnumerable<Pendencia>> GetAllAsync()
    {
        return await _context.Pendencias
            .Include(p => p.CartaoCredito)
            .Include(p => p.Parcelas)
            .Include(p => p.Acordos)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();
    }
    
    public async Task<Pendencia> GetByIdAsync(int id)
    {
        return await _context.Pendencias
            .Include(p => p.CartaoCredito)
            .Include(p => p.Parcelas)
            .Include(p => p.Acordos)
                .ThenInclude(a => a.Parcelas)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<IEnumerable<Pendencia>> GetByStatusAsync(StatusPendencia status)
    {
        return await _context.Pendencias
            .Include(p => p.CartaoCredito)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Pendencia>> GetAtradasAsync()
    {
        return await _context.Pendencias
            .Include(p => p.CartaoCredito)
            .Include(p => p.Parcelas)
            .Where(p => p.Status == StatusPendencia.Atrasada)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Pendencia>> GetByCartaoAsync(int cartaoId)
    {
        return await _context.Pendencias
            .Include(p => p.CartaoCredito)
            .Where(p => p.CartaoCreditoId == cartaoId)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();
    }
    
    public async Task<Pendencia> AddAsync(Pendencia pendencia)
    {
        _context.Pendencias.Add(pendencia);
        await _context.SaveChangesAsync();
        return pendencia;
    }
    
    public async Task UpdateAsync(Pendencia pendencia)
    {
        _context.Pendencias.Update(pendencia);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var pendencia = await _context.Pendencias.FindAsync(id);
        if (pendencia != null)
        {
            _context.Pendencias.Remove(pendencia);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<decimal> GetTotalDividasAsync()
    {
        return await _context.Pendencias
            .Where(p => p.Status != StatusPendencia.Quitada)
            .SumAsync(p => p.ValorTotal);
    }
    
    public async Task<decimal> GetTotalPagoAsync()
    {
        return await _context.Parcelas
            .Where(p => p.Status == StatusParcela.Paga)
            .SumAsync(p => p.Valor);
    }
    
    public async Task<int> GetQuantidadeAtradasAsync()
    {
        return await _context.Pendencias
            .CountAsync(p => p.Status == StatusPendencia.Atrasada);
    }
}
