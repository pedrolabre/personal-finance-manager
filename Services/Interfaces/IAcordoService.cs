
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PersonalFinanceManager.Models.DTOs;

namespace PersonalFinanceManager.Services.Interfaces;

#nullable enable
public interface IAcordoService
{
    Task<IEnumerable<AcordoDto>> ListarTodosAsync();
    Task<IEnumerable<AcordoDto>> ListarPorPendenciaAsync(int pendenciaId);
    Task<AcordoDto?> ObterPorIdAsync(int id);
    Task<AcordoDto> CriarAsync(AcordoDto dto);
    Task AtualizarAsync(int id, AcordoDto dto);
    Task ExcluirAsync(int id);
    Task<List<ParcelaDto>> GerarParcelasAsync(int acordoId, int numeroParcelas, decimal valorTotal, DateTime dataInicio);
}
#nullable restore
