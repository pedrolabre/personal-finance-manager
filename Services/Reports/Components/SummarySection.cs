using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PersonalFinanceManager.Services.Reports.Components
{
    /// <summary>
    /// Componente para seções de resumo com fundo colorido e múltiplas linhas de informação
    /// </summary>
    public class SummarySection : BaseReportComponent
    {
        private readonly string _titulo;
        private readonly ReportComposite _conteudo = new();
        
        public SummarySection(string titulo)
        {
            _titulo = titulo;
        }
        
        /// <summary>
        /// Adiciona uma linha de texto ao resumo
        /// </summary>
        public SummarySection AddLine(string texto, string cor = null)
        {
            _conteudo.Add(new TextLineComponent(texto, cor));
            return this;
        }
        
        /// <summary>
        /// Adiciona um componente customizado ao resumo
        /// </summary>
        public SummarySection AddComponent(IReportComponent component)
        {
            _conteudo.Add(component);
            return this;
        }
        
        public override void Compose(IContainer container)
        {
            container.Background(Colors.Grey.Lighten4).Padding(15).Column(column =>
            {
                column.Item()
                    .Text(_titulo)
                    .FontSize(14)
                    .Bold();
                    
                column.Item()
                    .PaddingTop(10)
                    .Element(_conteudo.Compose);
            });
        }
    }
    
    /// <summary>
    /// Componente auxiliar para linhas de texto simples
    /// </summary>
    internal class TextLineComponent : BaseReportComponent
    {
        private readonly string _texto;
        private readonly string _cor;
        
        public TextLineComponent(string texto, string cor = null)
        {
            _texto = texto;
            _cor = cor;
        }
        
        public override void Compose(IContainer container)
        {
            var textElement = container.Text(_texto);
            
            if (!string.IsNullOrEmpty(_cor))
            {
                textElement.FontColor(_cor);
            }
        }
    }
}
