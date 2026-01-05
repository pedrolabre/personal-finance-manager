#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using PersonalFinanceManager.Data.Entities;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Repositories.Interfaces;

public interface IParcelaRepository
{
    Task<IEnumerable<Parcela>> GetAllAsync();
    Task<IEnumerable<Parcela>> GetByPendenciaAsync(int pendenciaId);
    Task<IEnumerable<Parcela>> GetByAcordoAsync(int acordoId);
    Task<Parcela?> GetByIdAsync(int id);
    Task<IEnumerable<Parcela>> GetProximosVencimentosAsync(int dias);
    Task<IEnumerable<Parcela>> GetByStatusAsync(StatusParcela status);
    Task<Parcela> AddAsync(Parcela parcela);
    Task UpdateAsync(Parcela parcela);
    Task DeleteAsync(int id);
    Task MarcarComoPagaAsync(int id, DateTime dataPagamento);
}
