#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalFinanceManager.Models.DTOs;

namespace PersonalFinanceManager.Services.Interfaces;

public interface ICartaoCreditoService
{
    Task<IEnumerable<CartaoCreditoDto>> ListarTodosAsync();
    Task<IEnumerable<CartaoCreditoDto>> ListarAtivosAsync();
    Task<CartaoCreditoDto?> ObterPorIdAsync(int id);
    Task<CartaoCreditoDto> CriarAsync(CartaoCreditoDto dto);
    Task AtualizarAsync(int id, CartaoCreditoDto dto);
    Task ExcluirAsync(int id);
    Task<bool> ValidarNomeUnicoAsync(string nome, int? ignorarId = null);
}
