
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalFinanceManager.Services.Import.Models;

namespace PersonalFinanceManager.Services.Import.Parsers
{
    public class BankStatementParser : ITextParser
    {
        public IEnumerable<ImportedPendencia> Parse(string input)
        {
            // Implementação futura
            yield break;
        }

        public bool PodeProcessar(string texto)
        {
            // Implementação futura
            return false;
        }

        public Task<ImportResult> ParseAsync(string texto, ImportFormat formato)
        {
            // Implementação futura
            return Task.FromResult<ImportResult>(null);
        }

        public IEnumerable<ImportedPendencia> ValidarEPreparar(ImportResult result)
        {
            // Implementação futura
            yield break;
        }

        // Método legado removido
    }
}
