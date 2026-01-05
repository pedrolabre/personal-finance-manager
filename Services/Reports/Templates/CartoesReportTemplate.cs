using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PersonalFinanceManager.Data.Entities;
using PersonalFinanceManager.Services.Reports.Models;
using PersonalFinanceManager.Services.Reports.Components;

namespace PersonalFinanceManager.Services.Reports.Templates
{
    /// <summary>
    /// Template de relatório de Cartões de Crédito usando Composite Pattern
    /// </summary>
    public class CartoesReportTemplate
    {
        private readonly IEnumerable<CartaoCredito> _cartoes;
        private readonly ReportOptions _options;
        
        public CartoesReportTemplate(IEnumerable<CartaoCredito> cartoes, ReportOptions options)
        {
            _cartoes = cartoes?.ToList() ?? new List<CartaoCredito>();
            _options = options;
        }

        public void GeneratePdf(string outputPath)
        {
            var cartoesList = _cartoes.ToList();
            
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    
                    page.Header().Element(c => new HeaderComponent("Relatório de Cartões de Crédito").Compose(c));
                    page.Content().Element(c => BuildContent(cartoesList).Compose(c));
                    page.Footer().Element(c => new FooterComponent().Compose(c));
                });
            }).GeneratePdf(outputPath);
        }
        
        private IReportComponent BuildContent(List<CartaoCredito> cartoes)
        {
            if (!cartoes.Any())
            {
                return new EmptyContentComponent("Não há cartões cadastrados para exibir neste relatório.");
            }
            
            var composite = new ReportComposite();
            composite.Add(BuildResumo(cartoes));
            
            // Inclui gráficos se a opção estiver habilitada
            if (_options?.IncluirGraficos == true)
            {
                composite.Add(new PaddingComponent(BuildGraficos(cartoes), bottom: 15));
            }
            
            // Só inclui os cards detalhados se a opção estiver habilitada
            if (_options?.IncluirDetalhes == true)
            {
                // Adicionar card de cada cartão
                foreach (var cartao in cartoes)
                {
                    composite.Add(new PaddingComponent(new CartaoCard(cartao), bottom: 10));
                }
            }
            
            return composite;
        }
        
        private IReportComponent BuildResumo(List<CartaoCredito> cartoes)
        {
            var totalLimite = cartoes.Sum(c => c.Limite);
            var totalCartoes = cartoes.Count;
            var ativos = cartoes.Count(c => c.Ativo);
            
            var resumo = new SummarySection("Resumo Geral");
            resumo.AddLine($"Total de Cartões: {totalCartoes}   |   Cartões Ativos: {ativos}   |   Limite Total: {totalLimite:C2}");
            
            return new PaddingComponent(resumo, bottom: 15);
        }
        
        private IReportComponent BuildGraficos(List<CartaoCredito> cartoes)
        {
            var composite = new ReportComposite();
            
            // Gráfico de limites por cartão (apenas cartões ativos com limite definido)
            var cartoesAtivos = cartoes.Where(c => c.Ativo && c.Limite.HasValue).ToList();
            
            if (cartoesAtivos.Any())
            {
                var porCartao = cartoesAtivos
                    .Select(c => new BarChartItem
                    {
                        Label = c.Nome ?? "Sem nome",
                        Valor = c.Limite.GetValueOrDefault(),
                        Cor = Colors.Blue.Medium
                    })
                    .OrderByDescending(i => i.Valor)
                    .ToList();
                
                composite.Add(new PaddingComponent(
                    new BarChartComponent("Limite por Cartão (Ativos)", porCartao), 
                    bottom: 15));
            }
            
            // Gráfico por Banco
            var porBanco = cartoes
                .Where(c => c.Ativo && c.Limite.HasValue && !string.IsNullOrEmpty(c.Banco))
                .GroupBy(c => c.Banco)
                .Select(g => new BarChartItem
                {
                    Label = g.Key ?? "Sem banco",
                    Valor = g.Sum(c => c.Limite.GetValueOrDefault()),
                    Cor = Colors.Green.Medium
                })
                .OrderByDescending(i => i.Valor)
                .ToList();
            
            if (porBanco.Any())
            {
                composite.Add(new PaddingComponent(
                    new BarChartComponent("Limite Total por Banco", porBanco), 
                    bottom: 15));
            }
            
            return composite;
        }
    }
    
    /// <summary>
    /// Componente para exibir detalhes de um cartão de crédito
    /// </summary>
    internal class CartaoCard : BaseReportComponent
    {
        private readonly CartaoCredito _cartao;
        
        public CartaoCard(CartaoCredito cartao)
        {
            _cartao = cartao;
        }
        
        public override void Compose(IContainer container)
        {
            var statusColor = _cartao.Ativo ? Colors.Green.Medium : Colors.Red.Medium;
            var statusText = _cartao.Ativo ? "Ativo" : "Inativo";
            
            container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(_cartao.Nome ?? "Sem nome").FontSize(14).Bold();
                    row.ConstantItem(60).AlignRight().Text(statusText).FontColor(statusColor).Bold();
                });
                
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Text($"Banco: {_cartao.Banco ?? "-"}");
                    row.RelativeItem().Text($"Limite: {_cartao.Limite:C2}");
                });
                
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Dia Fechamento: {_cartao.DiaFechamento}");
                    row.RelativeItem().Text($"Dia Vencimento: {_cartao.DiaVencimento}");
                });
            });
        }
    }
}
