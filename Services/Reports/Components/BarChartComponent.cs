#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PersonalFinanceManager.Services.Reports.Components
{
    /// <summary>
    /// Componente para gráfico de barras horizontal
    /// </summary>
    public class BarChartComponent : BaseReportComponent
    {
        private readonly string _titulo;
        private readonly List<BarChartItem> _items;
        private readonly decimal _maxValue;

        public BarChartComponent(string titulo, List<BarChartItem> items)
        {
            _titulo = titulo;
            _items = items ?? new List<BarChartItem>();
            _maxValue = _items.Any() ? _items.Max(i => i.Valor) : 0;
        }

        public override void Compose(IContainer container)
        {
            if (!_items.Any())
            {
                return;
            }

            container.Column(column =>
            {
                // Título do gráfico
                column.Item()
                    .PaddingBottom(10)
                    .Text(_titulo)
                    .FontSize(12)
                    .Bold()
                    .FontColor(Colors.Grey.Darken2);

                // Barras
                foreach (var item in _items)
                {
                    column.Item().PaddingBottom(8).Row(row =>
                    {
                        // Label (30% da largura)
                        row.ConstantItem(120)
                            .AlignMiddle()
                            .Text(item.Label)
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);

                        // Barra e valor
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Row(barRow =>
                            {
                                // Barra proporcional
                                var percentage = _maxValue > 0 ? (float)(item.Valor / _maxValue) : 0;
                                var barWidth = Math.Max(percentage * 100, 1); // Mínimo 1% para visibilidade

                                barRow.RelativeItem(barWidth / 100f)
                                    .Height(20)
                                    .Background(item.Cor ?? Colors.Blue.Medium)
                                    .AlignMiddle()
                                    .PaddingHorizontal(5)
                                    .Text(item.Valor.ToString("C2"))
                                    .FontSize(8)
                                    .FontColor(Colors.White)
                                    .Bold();
                            });
                        });
                    });
                }

                // Linha de separação
                column.Item()
                    .PaddingTop(10)
                    .LineHorizontal(1)
                    .LineColor(Colors.Grey.Lighten2);
            });
        }
    }

    /// <summary>
    /// Item de dados para o gráfico de barras
    /// </summary>
    public class BarChartItem
    {
        public string Label { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string? Cor { get; set; }
    }
}
