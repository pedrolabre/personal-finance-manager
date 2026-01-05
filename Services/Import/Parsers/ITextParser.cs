using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalFinanceManager.Services.Import.Models;

namespace PersonalFinanceManager.Services.Import.Parsers
{
    public interface ITextParser
    {
        bool PodeProcessar(string texto);
        Task<ImportResult> ParseAsync(string texto, ImportFormat formato);
        IEnumerable<ImportedPendencia> ValidarEPreparar(ImportResult result);
    }
}
