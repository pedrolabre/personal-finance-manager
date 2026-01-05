using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using PersonalFinanceManager.Data.Entities;

namespace PersonalFinanceManager.Repositories.Interfaces;

public interface ICartaoCreditoRepository
{
    Task<IEnumerable<CartaoCredito>> GetAllAsync();
    Task<IEnumerable<CartaoCredito>> GetAtivosAsync();
    Task<CartaoCredito> GetByIdAsync(int id);
    Task<CartaoCredito> GetByNomeAsync(string nome);
    Task<CartaoCredito> AddAsync(CartaoCredito cartao);
    Task UpdateAsync(CartaoCredito cartao);
    Task DeleteAsync(int id);
    Task<bool> ExisteCartaoComNomeAsync(string nome, int ignorarId = 0);
}
