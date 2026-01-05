using System;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PersonalFinanceManager.Models.DTOs;
using PersonalFinanceManager.Services.Reports.Components;

namespace PersonalFinanceManager.Services.Reports.Templates
{
    /// <summary>
    /// Template de relatório de Dashboard usando Composite Pattern
    /// </summary>
    public class DashboardReportTemplate
    {
        private readonly DashboardResumoDto _resumo;
        
        public DashboardReportTemplate(DashboardResumoDto resumo)
        {
            _resumo = resumo;
        }

        public void GeneratePdf(string outputPath)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    
                    page.Header().Element(c => new HeaderComponent("Dashboard Completo - Resumo Financeiro").Compose(c));
                    page.Content().Element(c => BuildContent().Compose(c));
                    page.Footer().Element(c => new FooterComponent().Compose(c));
                });
            }).GeneratePdf(outputPath);
        }
        
        private IReportComponent BuildContent()
        {
            if (_resumo == null)
            {
                return new EmptyContentComponent("Não há dados disponíveis para exibir neste relatório.");
            }
            
            var composite = new ReportComposite();
            
            composite.Add(new PaddingComponent(BuildCardsResumo(), bottom: 20));
            composite.Add(new PaddingComponent(BuildEstatisticas(), bottom: 20));
            
            if (_resumo.ResumoCartoes?.Any() == true)
            {
                composite.Add(new PaddingComponent(BuildCartoesTabela(), bottom: 20));
            }
            
            if (_resumo.ProximosVencimentos?.Any() == true)
            {
                composite.Add(BuildProximosVencimentosTabela());
            }
            
            return composite;
        }
        
        private IReportComponent BuildCardsResumo()
        {
            return new DashboardCardsComponent(_resumo);
        }
        
        private IReportComponent BuildEstatisticas()
        {
            var resumo = new SummarySection("Estatísticas");
            resumo.AddLine($"Pendências Totais: {_resumo.QuantidadePendencias}");
            resumo.AddLine($"Pendências Atrasadas: {_resumo.QuantidadePendenciasAtrasadas}", Colors.Red.Medium);
            resumo.AddLine($"Próximos Vencimentos (7 dias): {_resumo.QuantidadeParcelasProximosVencimentos}");
            resumo.AddLine($"Valor Próx. Vencimentos: {_resumo.ValorProximosVencimentos:C2}", Colors.Orange.Medium);
            resumo.AddLine($"Recebimentos Esperados: {_resumo.TotalRecebimentosEsperados:C2}");
            resumo.AddLine($"Recebimentos Recebidos: {_resumo.TotalRecebimentosRecebidos:C2}", Colors.Green.Medium);
            
            return resumo;
        }
        
        private IReportComponent BuildCartoesTabela()
        {
            var tabela = new TableComponent()
                .AddColumn("Cartão", 3)
                .AddColumn("Limite", 2)
                .AddColumn("Utilizado", 2)
                .AddColumn("Disponível", 2);
                
            foreach (var cartao in _resumo.ResumoCartoes)
            {
                var disponivel = (cartao.Limite ?? 0) - cartao.TotalDividas;
                tabela.AddRow(
                    cartao.Nome ?? "Sem nome",
                    $"{cartao.Limite:C2}",
                    $"{cartao.TotalDividas:C2}",
                    $"{disponivel:C2}"
                );
            }
            
            return new TitledComponent("Cartões de Crédito", tabela);
        }
        
        private IReportComponent BuildProximosVencimentosTabela()
        {
            var tabela = new TableComponent(Colors.Orange.Darken2)
                .AddColumn("Descrição", 4)
                .AddColumn("Vencimento", 2)
                .AddColumn("Valor", 2);
                
            foreach (var parcela in _resumo.ProximosVencimentos.Take(10))
            {
                tabela.AddRow(
                    parcela.NomePendencia ?? "Sem descrição",
                    parcela.DataVencimento.ToString("dd/MM/yyyy"),
                    $"{parcela.Valor:C2}"
                );
            }
            
            return new TitledComponent("Próximos Vencimentos", tabela);
        }
    }
    
    /// <summary>
    /// Componente para os cards de resumo financeiro do dashboard
    /// </summary>
    internal class DashboardCardsComponent : BaseReportComponent
    {
        private readonly DashboardResumoDto _resumo;
        
        public DashboardCardsComponent(DashboardResumoDto resumo)
        {
            _resumo = resumo;
        }
        
        public override void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("Resumo Financeiro").FontSize(14).Bold();
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Background(Colors.Red.Lighten4).Padding(15).Column(c =>
                    {
                        c.Item().Text("Total de Dívidas").FontSize(10).FontColor(Colors.Grey.Darken2);
                        c.Item().Text($"{_resumo.TotalDividas:C2}").FontSize(16).Bold().FontColor(Colors.Red.Darken2);
                    });
                    
                    row.ConstantItem(10);
                    
                    row.RelativeItem().Background(Colors.Green.Lighten4).Padding(15).Column(c =>
                    {
                        c.Item().Text("Total Pago").FontSize(10).FontColor(Colors.Grey.Darken2);
                        c.Item().Text($"{_resumo.TotalPago:C2}").FontSize(16).Bold().FontColor(Colors.Green.Darken2);
                    });
                    
                    row.ConstantItem(10);
                    
                    row.RelativeItem().Background(Colors.Orange.Lighten4).Padding(15).Column(c =>
                    {
                        c.Item().Text("Restante").FontSize(10).FontColor(Colors.Grey.Darken2);
                        c.Item().Text($"{_resumo.TotalRestante:C2}").FontSize(16).Bold().FontColor(Colors.Orange.Darken2);
                    });
                });
                
                column.Item().PaddingTop(15).Background(Colors.Blue.Lighten4).Padding(15).Column(c =>
                {
                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Progresso de Pagamento").FontSize(12).Bold();
                        r.ConstantItem(80).AlignRight().Text($"{_resumo.PercentualPago:F1}%").FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
                    });
                });
            });
        }
    }
    
    /// <summary>
    /// Componente que adiciona um título acima de outro componente
    /// </summary>
    internal class TitledComponent : BaseReportComponent
    {
        private readonly string _titulo;
        private readonly IReportComponent _child;
        
        public TitledComponent(string titulo, IReportComponent child)
        {
            _titulo = titulo;
            _child = child;
        }
        
        public override bool HasContent => _child.HasContent;
        
        public override void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text(_titulo).FontSize(14).Bold();
                column.Item().PaddingTop(10).Element(_child.Compose);
            });
        }
    }
}

