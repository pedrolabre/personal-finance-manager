using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalFinanceManager.Data.Entities;

namespace PersonalFinanceManager.Repositories.Interfaces;

public interface IPendenciaRepository
{
    Task<IEnumerable<Pendencia>> GetAllAsync();
    Task<Pendencia> GetByIdAsync(int id);
    Task<IEnumerable<Pendencia>> GetByStatusAsync(Models.Enums.StatusPendencia status);
    Task<IEnumerable<Pendencia>> GetAtradasAsync();
    Task<IEnumerable<Pendencia>> GetByCartaoAsync(int cartaoId);
    Task<Pendencia> AddAsync(Pendencia pendencia);
    Task UpdateAsync(Pendencia pendencia);
    Task DeleteAsync(int id);
    Task<decimal> GetTotalDividasAsync();
    Task<decimal> GetTotalPagoAsync();
    Task<int> GetQuantidadeAtradasAsync();
}
