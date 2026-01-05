using QuestPDF.Infrastructure;

namespace PersonalFinanceManager.Services.Reports.Components
{
    /// <summary>
    /// Interface base para componentes de relatório usando Composite Pattern
    /// </summary>
    public interface IReportComponent
    {
        /// <summary>
        /// Renderiza o componente no container fornecido
        /// </summary>
        void Compose(IContainer container);
        
        /// <summary>
        /// Indica se o componente possui dados para renderizar
        /// </summary>
        bool HasContent { get; }
    }
}
