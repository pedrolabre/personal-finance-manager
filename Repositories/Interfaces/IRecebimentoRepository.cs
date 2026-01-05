using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using PersonalFinanceManager.Data.Entities;

namespace PersonalFinanceManager.Repositories.Interfaces;

public interface IRecebimentoRepository
{
    Task<IEnumerable<Recebimento>> GetAllAsync();
    Task<Recebimento> GetByIdAsync(int id);
    Task<IEnumerable<Recebimento>> GetPendentesAsync();
    Task<IEnumerable<Recebimento>> GetAtrasadosAsync();
    Task<IEnumerable<Recebimento>> GetByMesAsync(int ano, int mes);
    Task<Recebimento> AddAsync(Recebimento recebimento);
    Task UpdateAsync(Recebimento recebimento);
    Task DeleteAsync(int id);
    Task<decimal> GetTotalEsperadoAsync();
    Task<decimal> GetTotalRecebidoAsync();
}
