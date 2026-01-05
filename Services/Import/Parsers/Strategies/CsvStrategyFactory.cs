#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace PersonalFinanceManager.Services.Import.Parsers.Strategies
{
    /// <summary>
    /// Factory para selecionar a estratégia de parsing apropriada
    /// </summary>
    public class CsvStrategyFactory
    {
        private readonly IEnumerable<ICsvParsingStrategy> _strategies;
        
        public CsvStrategyFactory(IEnumerable<ICsvParsingStrategy> strategies)
        {
            _strategies = strategies;
        }
        
        /// <summary>
        /// Seleciona a melhor estratégia para o conteúdo CSV fornecido
        /// </summary>
        public ICsvParsingStrategy SelectStrategy(string csvContent, char separator)
        {
            // Tentar estratégias específicas primeiro
            var specificStrategy = _strategies
                .Where(s => s.StrategyName != "Generic CSV")
                .FirstOrDefault(s => s.CanHandle(csvContent, separator));
            
            if (specificStrategy != null)
                return specificStrategy;
            
            // Fallback para estratégia genérica
            return _strategies.First(s => s.StrategyName == "Generic CSV");
        }
        
        /// <summary>
        /// Retorna todas as estratégias disponíveis
        /// </summary>
        public IEnumerable<ICsvParsingStrategy> GetAllStrategies() => _strategies;
    }
}
