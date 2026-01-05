using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PersonalFinanceManager.Services.Reports.Components
{
    /// <summary>
    /// Componente reutilizável para rodapé de relatórios com paginação
    /// </summary>
    public class FooterComponent : BaseReportComponent
    {
        private readonly string _textoAdicional;
        
        public FooterComponent(string textoAdicional = "Personal Finance Manager")
        {
            _textoAdicional = textoAdicional;
        }
        
        public override void Compose(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.Span($"{_textoAdicional} - ");
                text.CurrentPageNumber();
                text.Span(" / ");
                text.TotalPages();
            });
        }
    }
}
