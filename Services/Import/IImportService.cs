
using System.Threading.Tasks;
using PersonalFinanceManager.Services.Import.Models;

namespace PersonalFinanceManager.Services.Import
{
    public interface IImportService
    {
        Task<ImportResult> ImportarAsync(string input, ImportFormat format);
        Task<ImportResult> ValidarTextoAsync(string texto, ImportFormat formato);
        Task<ImportResult> ImportarDeTextoAsync(string texto, ImportFormat formato);
    }
}
