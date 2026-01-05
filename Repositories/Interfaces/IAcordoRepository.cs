#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalFinanceManager.Data.Entities;

namespace PersonalFinanceManager.Repositories.Interfaces;

public interface IAcordoRepository
{
    Task<IEnumerable<Acordo>> GetAllAsync();
    Task<IEnumerable<Acordo>> GetByPendenciaAsync(int pendenciaId);
    Task<Acordo?> GetByIdAsync(int id);
    Task<Acordo?> GetAcordoAtivoByPendenciaAsync(int pendenciaId);
    Task<Acordo> AddAsync(Acordo acordo);
    Task UpdateAsync(Acordo acordo);
    Task DeleteAsync(int id);
    Task DesativarAcordosAnterioresAsync(int pendenciaId);
}
