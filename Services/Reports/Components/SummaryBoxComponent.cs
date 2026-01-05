using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PersonalFinanceManager.Services.Reports.Components
{
    /// <summary>
    /// Componente para caixas de resumo (cards) com título e valor
    /// </summary>
    public class SummaryBoxComponent : BaseReportComponent
    {
        private readonly string _titulo;
        private readonly string _valor;
        private readonly string _backgroundColor;
        private readonly string _textColor;
        
        public SummaryBoxComponent(
            string titulo, 
            string valor, 
            string backgroundColor = "#F5F5F5", 
            string textColor = "#424242")
        {
            _titulo = titulo;
            _valor = valor;
            _backgroundColor = backgroundColor;
            _textColor = textColor;
        }
        
        public override void Compose(IContainer container)
        {
            container.Background(_backgroundColor).Padding(15).Column(column =>
            {
                column.Item()
                    .Text(_titulo)
                    .FontSize(10)
                    .FontColor(_textColor);
                    
                column.Item()
                    .Text(_valor)
                    .FontSize(16)
                    .Bold()
                    .FontColor(_textColor);
            });
        }
    }
}
