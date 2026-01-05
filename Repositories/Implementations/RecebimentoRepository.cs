using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Data.Entities;
using PersonalFinanceManager.Repositories.Interfaces;

namespace PersonalFinanceManager.Repositories.Implementations;

public class RecebimentoRepository : IRecebimentoRepository
{
    private readonly AppDbContext _context;
    
    public RecebimentoRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<IEnumerable<Recebimento>> GetAllAsync()
    {
        return await _context.Recebimentos
            .OrderByDescending(r => r.DataPrevista)
            .ToListAsync();
    }
    
    public async Task<Recebimento> GetByIdAsync(int id)
    {
        return await _context.Recebimentos.FindAsync(id);
    }
    
    public async Task<IEnumerable<Recebimento>> GetPendentesAsync()
    {
        return await _context.Recebimentos
            .Where(r => !r.RecebimentoCompleto)
            .OrderBy(r => r.DataPrevista)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Recebimento>> GetAtrasadosAsync()
    {
        return await _context.Recebimentos
            .Where(r => !r.RecebimentoCompleto && r.DataPrevista < DateTime.Now)
            .OrderBy(r => r.DataPrevista)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Recebimento>> GetByMesAsync(int ano, int mes)
    {
        return await _context.Recebimentos
            .Where(r => r.DataPrevista.Year == ano && r.DataPrevista.Month == mes)
            .OrderBy(r => r.DataPrevista)
            .ToListAsync();
    }
    
    public async Task<Recebimento> AddAsync(Recebimento recebimento)
    {
        _context.Recebimentos.Add(recebimento);
        await _context.SaveChangesAsync();
        return recebimento;
    }
    
    public async Task UpdateAsync(Recebimento recebimento)
    {
        _context.Recebimentos.Update(recebimento);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var recebimento = await _context.Recebimentos.FindAsync(id);
        if (recebimento != null)
        {
            _context.Recebimentos.Remove(recebimento);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<decimal> GetTotalEsperadoAsync()
    {
        return await _context.Recebimentos
            .Where(r => !r.RecebimentoCompleto)
            .SumAsync(r => r.ValorEsperado);
    }
    
    public async Task<decimal> GetTotalRecebidoAsync()
    {
        return await _context.Recebimentos
            .SumAsync(r => r.ValorRecebido);
    }
}
