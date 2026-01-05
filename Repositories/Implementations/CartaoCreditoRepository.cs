using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Data.Entities;
using PersonalFinanceManager.Repositories.Interfaces;

namespace PersonalFinanceManager.Repositories.Implementations;

public class CartaoCreditoRepository : ICartaoCreditoRepository
{
    private readonly AppDbContext _context;
    
    public CartaoCreditoRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<IEnumerable<CartaoCredito>> GetAllAsync()
    {
        return await _context.CartoesCredito
            .Include(c => c.Pendencias)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<CartaoCredito>> GetAtivosAsync()
    {
        return await _context.CartoesCredito
            .Include(c => c.Pendencias)
            .Where(c => c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }
    
    public async Task<CartaoCredito> GetByIdAsync(int id)
    {
        return await _context.CartoesCredito
            .Include(c => c.Pendencias)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<CartaoCredito> GetByNomeAsync(string nome)
    {
        return await _context.CartoesCredito
            .FirstOrDefaultAsync(c => c.Nome.ToLower() == nome.ToLower());
    }
    
    public async Task<CartaoCredito> AddAsync(CartaoCredito cartao)
    {
        _context.CartoesCredito.Add(cartao);
        await _context.SaveChangesAsync();
        return cartao;
    }
    
    public async Task UpdateAsync(CartaoCredito cartao)
    {
        _context.CartoesCredito.Update(cartao);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var cartao = await _context.CartoesCredito.FindAsync(id);
        if (cartao != null)
        {
            _context.CartoesCredito.Remove(cartao);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<bool> ExisteCartaoComNomeAsync(string nome, int ignorarId = 0)
    {
        var query = _context.CartoesCredito
            .Where(c => c.Nome.ToLower() == nome.ToLower());
        if (ignorarId != 0)
        {
            query = query.Where(c => c.Id != ignorarId);
        }
        return await query.AnyAsync();
    }
}
