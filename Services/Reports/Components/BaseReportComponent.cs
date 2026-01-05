using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PersonalFinanceManager.Services.Reports.Components
{
    /// <summary>
    /// Classe base abstrata para componentes de relatório
    /// Fornece funcionalidade comum para todos os componentes
    /// </summary>
    public abstract class BaseReportComponent : IReportComponent
    {
        public abstract void Compose(IContainer container);
        
        public virtual bool HasContent => true;
        
        /// <summary>
        /// Renderiza mensagem padrão quando não há conteúdo
        /// </summary>
        protected void ComposeEmptyMessage(IContainer container, string message = "Não há dados disponíveis para exibir.")
        {
            container.PaddingVertical(20)
                .Text(message)
                .FontSize(14)
                .FontColor(QuestPDF.Helpers.Colors.Grey.Medium)
                .Italic();
        }
    }
}
