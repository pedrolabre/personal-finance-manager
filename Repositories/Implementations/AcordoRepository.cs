using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Data.Entities;
using PersonalFinanceManager.Repositories.Interfaces;

namespace PersonalFinanceManager.Repositories.Implementations;

public class AcordoRepository : IAcordoRepository
{
    private readonly AppDbContext _context;
    
    public AcordoRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<IEnumerable<Acordo>> GetAllAsync()
    {
        return await _context.Acordos
            .OrderByDescending(a => a.DataAcordo)
            .Include(a => a.Pendencia)
            .Include(a => a.Parcelas)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Acordo>> GetByPendenciaAsync(int pendenciaId)
    {
        return await _context.Acordos
            .Where(a => a.PendenciaId == pendenciaId)
            .OrderByDescending(a => a.DataAcordo)
            .Include(a => a.Parcelas)
            .ToListAsync();
    }
    
    public async Task<Acordo> GetByIdAsync(int id)
    {
        return await _context.Acordos
            .Include(a => a.Pendencia)
            .Include(a => a.Parcelas)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
    
    public async Task<Acordo> GetAcordoAtivoByPendenciaAsync(int pendenciaId)
    {
        return await _context.Acordos
            .Include(a => a.Parcelas)
            .FirstOrDefaultAsync(a => a.PendenciaId == pendenciaId && a.Ativo);
    }
    
    public async Task<Acordo> AddAsync(Acordo acordo)
    {
        _context.Acordos.Add(acordo);
        await _context.SaveChangesAsync();
        return acordo;
    }
    
    public async Task UpdateAsync(Acordo acordo)
    {
        _context.Acordos.Update(acordo);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var acordo = await _context.Acordos.FindAsync(id);
        if (acordo != null)
        {
            _context.Acordos.Remove(acordo);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task DesativarAcordosAnterioresAsync(int pendenciaId)
    {
        var acordosAntigos = await _context.Acordos
            .Where(a => a.PendenciaId == pendenciaId && a.Ativo)
            .ToListAsync();
        
        foreach (var acordo in acordosAntigos)
        {
            acordo.Ativo = false;
        }
        
        await _context.SaveChangesAsync();
    }
}
