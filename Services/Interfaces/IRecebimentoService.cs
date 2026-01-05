using PersonalFinanceManager.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PersonalFinanceManager.Services.Interfaces;

public interface IRecebimentoService
{
    Task<IEnumerable<RecebimentoDto>> ListarTodosAsync();
    Task<IEnumerable<RecebimentoDto>> ListarPendentesAsync();
    Task<IEnumerable<RecebimentoDto>> ListarAtrasadosAsync();
    Task<RecebimentoDto> ObterPorIdAsync(int id);
    Task<RecebimentoDto> CriarAsync(RecebimentoDto dto);
    Task AtualizarAsync(int id, RecebimentoDto dto);
    Task ExcluirAsync(int id);
    Task RegistrarRecebimentoParcialAsync(int id, decimal valorRecebido);
    Task RegistrarRecebimentoCompletoAsync(int id, DateTime dataRecebimento);
}
