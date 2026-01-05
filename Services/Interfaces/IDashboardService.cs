using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalFinanceManager.Models.DTOs;

namespace PersonalFinanceManager.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResumoDto> ObterResumoAsync();
        Task<IEnumerable<ParcelaDto>> ObterProximosVencimentosAsync(int dias = 7);
        Task AtualizarStatusPendenciasAsync();
    }
}
