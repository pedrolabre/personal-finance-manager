using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PersonalFinanceManager.Services.Reports.Components
{
    /// <summary>
    /// Componente reutilizável para cabeçalho de relatórios
    /// </summary>
    public class HeaderComponent : BaseReportComponent
    {
        private readonly string _titulo;
        private readonly bool _incluirDataGeracao;
        
        public HeaderComponent(string titulo, bool incluirDataGeracao = true)
        {
            _titulo = titulo;
            _incluirDataGeracao = incluirDataGeracao;
        }
        
        public override void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item()
                    .Text(_titulo)
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);
                    
                if (_incluirDataGeracao)
                {
                    column.Item()
                        .Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Medium);
                }
                
                column.Item()
                    .PaddingVertical(10)
                    .LineHorizontal(1)
                    .LineColor(Colors.Grey.Lighten2);
            });
        }
    }
}
