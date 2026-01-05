using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PersonalFinanceManager.Services.Reports.Components
{
    /// <summary>
    /// Componente genérico para tabelas com cabeçalho e linhas
    /// </summary>
    public class TableComponent : BaseReportComponent
    {
        private readonly List<TableColumn> _columns = new();
        private readonly List<TableRow> _rows = new();
        private readonly string _headerColor;
        
        public override bool HasContent => _rows.Any();
        
        public TableComponent(string headerColor = null)
        {
            _headerColor = headerColor ?? Colors.Blue.Darken2;
        }
        
        /// <summary>
        /// Adiciona uma coluna à tabela
        /// </summary>
        public TableComponent AddColumn(string titulo, float relativeLargura = 1)
        {
            _columns.Add(new TableColumn { Titulo = titulo, RelativeLargura = relativeLargura });
            return this;
        }
        
        /// <summary>
        /// Adiciona uma linha de dados à tabela
        /// </summary>
        public TableComponent AddRow(params string[] valores)
        {
            _rows.Add(new TableRow { Valores = valores.ToList() });
            return this;
        }
        
        public override void Compose(IContainer container)
        {
            if (!HasContent)
            {
                ComposeEmptyMessage(container, "Não há dados para exibir na tabela.");
                return;
            }
            
            container.Table(table =>
            {
                // Definir colunas
                table.ColumnsDefinition(columns =>
                {
                    foreach (var coluna in _columns)
                    {
                        columns.RelativeColumn(coluna.RelativeLargura);
                    }
                });
                
                // Cabeçalho
                table.Header(header =>
                {
                    foreach (var coluna in _columns)
                    {
                        header.Cell()
                            .Background(_headerColor)
                            .Padding(5)
                            .Text(coluna.Titulo)
                            .FontColor(Colors.White)
                            .Bold();
                    }
                });
                
                // Linhas de dados
                var index = 0;
                foreach (var linha in _rows)
                {
                    var bgColor = index % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                    
                    foreach (var valor in linha.Valores)
                    {
                        table.Cell()
                            .Background(bgColor)
                            .Padding(5)
                            .Text(valor ?? "-");
                    }
                    
                    index++;
                }
            });
        }
        
        private class TableColumn
        {
            public string Titulo { get; set; }
            public float RelativeLargura { get; set; }
        }
        
        private class TableRow
        {
            public List<string> Valores { get; set; }
        }
    }
}
