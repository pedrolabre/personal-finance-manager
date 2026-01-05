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

public class ParcelaRepository : IParcelaRepository
{
    private readonly AppDbContext _context;
    
    public ParcelaRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<IEnumerable<Parcela>> GetAllAsync()
    {
        return await _context.Parcelas
            .Include(p => p.Pendencia)
            .Include(p => p.Acordo)
            .OrderBy(p => p.DataVencimento)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Parcela>> GetByPendenciaAsync(int pendenciaId)
    {
        return await _context.Parcelas
            .Where(p => p.PendenciaId == pendenciaId)
            .OrderBy(p => p.NumeroParcela)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Parcela>> GetByAcordoAsync(int acordoId)
    {
        return await _context.Parcelas
            .Where(p => p.AcordoId == acordoId)
            .OrderBy(p => p.NumeroParcela)
            .ToListAsync();
    }
    
    public async Task<Parcela> GetByIdAsync(int id)
    {
        return await _context.Parcelas
            .Include(p => p.Pendencia)
            .Include(p => p.Acordo)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<IEnumerable<Parcela>> GetProximosVencimentosAsync(int dias)
    {
        var hoje = DateTime.Today;
        var dataLimite = hoje.AddDays(dias).AddDays(1).AddTicks(-1); // Fim do dia limite
        
        return await _context.Parcelas
            .Include(p => p.Pendencia)
            .Where(p => p.Status == StatusParcela.Pendente 
                     && p.DataVencimento >= hoje 
                     && p.DataVencimento <= dataLimite)
            .OrderBy(p => p.DataVencimento)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Parcela>> GetByStatusAsync(StatusParcela status)
    {
        return await _context.Parcelas
            .Include(p => p.Pendencia)
            .Where(p => p.Status == status)
            .OrderBy(p => p.DataVencimento)
            .ToListAsync();
    }
    
    public async Task<Parcela> AddAsync(Parcela parcela)
    {
        _context.Parcelas.Add(parcela);
        await _context.SaveChangesAsync();
        return parcela;
    }
    
    public async Task UpdateAsync(Parcela parcela)
    {
        _context.Parcelas.Update(parcela);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var parcela = await _context.Parcelas.FindAsync(id);
        if (parcela != null)
        {
            _context.Parcelas.Remove(parcela);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task MarcarComoPagaAsync(int id, DateTime dataPagamento)
    {
        var parcela = await _context.Parcelas.FindAsync(id);
        if (parcela != null)
        {
            parcela.Status = StatusParcela.Paga;
            parcela.DataPagamento = dataPagamento;
            await _context.SaveChangesAsync();
        }
    }
}
