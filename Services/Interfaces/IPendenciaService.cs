#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalFinanceManager.Models.DTOs;

namespace PersonalFinanceManager.Services.Interfaces
{
    public interface IPendenciaService
    {
        Task<IEnumerable<PendenciaDto>> ListarTodasAsync();
        Task<PendenciaDto?> ObterPorIdAsync(int id);
        Task<IEnumerable<PendenciaDto>> ListarPorStatusAsync(Models.Enums.StatusPendencia status);
        Task<IEnumerable<PendenciaDto>> ListarAtradasAsync();
        Task<PendenciaDto> CriarAsync(PendenciaDto dto);
        Task AtualizarAsync(int id, PendenciaDto dto);
        Task QuitarAsync(int id);
        Task ExcluirAsync(int id);
        Task AtualizarStatusAsync(int id, Models.Enums.StatusPendencia novoStatus);
    }
}
