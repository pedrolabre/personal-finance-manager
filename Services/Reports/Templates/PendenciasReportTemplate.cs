using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PersonalFinanceManager.Data.Entities;
using PersonalFinanceManager.Services.Reports.Models;
using PersonalFinanceManager.Services.Reports.Components;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Services.Reports.Templates
{
    /// <summary>
    /// Template de relatório de Pendências usando Composite Pattern
    /// </summary>
    public class PendenciasReportTemplate
    {
        private readonly IEnumerable<Pendencia> _pendencias;
        private readonly ReportOptions _options;
        
        public PendenciasReportTemplate(IEnumerable<Pendencia> pendencias, ReportOptions options)
        {
            _pendencias = pendencias?.ToList() ?? new List<Pendencia>();
            _options = options;
        }

        public void GeneratePdf(string outputPath)
        {
            try
            {
                var pendenciasList = _pendencias.ToList();
                
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(30);
                        page.DefaultTextStyle(x => x.FontSize(10));
                        
                        page.Header().Element(c => BuildHeader().Compose(c));
                        page.Content().Element(c => BuildContent(pendenciasList).Compose(c));
                        page.Footer().Element(c => new FooterComponent().Compose(c));
                    });
                }).GeneratePdf(outputPath);
                
                System.IO.File.AppendAllText("log_geracao_relatorio.txt", $"[OK] PDF criado: {outputPath} - {DateTime.Now}\n");
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("log_geracao_relatorio.txt", $"[ERRO] Falha ao criar PDF: {outputPath} - {DateTime.Now} - {ex.Message}\n{ex.StackTrace}\n");
                throw;
            }
        }
        
        private IReportComponent BuildHeader()
        {
            var titulo = _options?.FiltrarPorStatus == StatusPendencia.Atrasada 
                ? "Relatório de Pendências Atrasadas" 
                : "Relatório de Todas as Pendências";
                
            return new HeaderComponent(titulo);
        }
        
        private IReportComponent BuildContent(List<Pendencia> pendencias)
        {
            if (!pendencias.Any())
            {
                return new EmptyContentComponent("Não há pendências para exibir neste relatório.");
            }
            
            var composite = new ReportComposite();
            composite.Add(BuildResumo(pendencias));
            
            // Inclui gráficos se a opção estiver habilitada
            if (_options?.IncluirGraficos == true)
            {
                composite.Add(new PaddingComponent(BuildGraficos(pendencias), bottom: 15));
            }
            
            // Só inclui a tabela detalhada se a opção estiver habilitada
            if (_options?.IncluirDetalhes == true)
            {
                composite.Add(BuildTabela(pendencias));
            }
            
            return composite;
        }
        
        private IReportComponent BuildResumo(List<Pendencia> pendencias)
        {
            var totalValor = pendencias.Sum(p => p.ValorTotal);
            var totalPendencias = pendencias.Count;
            var atrasadas = pendencias.Count(p => p.Status == StatusPendencia.Atrasada);
            var emAberto = pendencias.Count(p => p.Status == StatusPendencia.EmAberto);
            
            var resumo = new SummarySection("Resumo");
            resumo.AddLine($"Total de Pendências: {totalPendencias}   |   Valor Total: {totalValor:C2}");
            resumo.AddLine($"Em Aberto: {emAberto}", Colors.Orange.Medium);
            resumo.AddLine($"Atrasadas: {atrasadas}", Colors.Red.Medium);
            
            return new PaddingComponent(resumo, bottom: 15);
        }
        
        private IReportComponent BuildGraficos(List<Pendencia> pendencias)
        {
            var composite = new ReportComposite();
            
            // Gráfico por Status
            var porStatus = pendencias
                .GroupBy(p => p.Status)
                .Select(g => new BarChartItem
                {
                    Label = g.Key.ToString(),
                    Valor = g.Sum(p => p.ValorTotal),
                    Cor = g.Key switch
                    {
                        StatusPendencia.EmAberto => Colors.Orange.Medium,
                        StatusPendencia.Atrasada => Colors.Red.Medium,
                        StatusPendencia.Quitada => Colors.Green.Medium,
                        StatusPendencia.Acordada => Colors.Blue.Medium,
                        _ => Colors.Grey.Medium
                    }
                })
                .OrderByDescending(i => i.Valor)
                .ToList();
            
            if (porStatus.Any())
            {
                composite.Add(new PaddingComponent(
                    new BarChartComponent("Distribuição por Status", porStatus), 
                    bottom: 15));
            }
            
            // Gráfico por Tipo de Dívida
            var porTipo = pendencias
                .GroupBy(p => p.TipoDivida)
                .Select(g => new BarChartItem
                {
                    Label = g.Key.ToString(),
                    Valor = g.Sum(p => p.ValorTotal),
                    Cor = Colors.Blue.Medium
                })
                .OrderByDescending(i => i.Valor)
                .ToList();
            
            if (porTipo.Any())
            {
                composite.Add(new PaddingComponent(
                    new BarChartComponent("Distribuição por Tipo de Dívida", porTipo), 
                    bottom: 15));
            }
            
            return composite;
        }
        
        private IReportComponent BuildTabela(List<Pendencia> pendencias)
        {
            var tabela = new TableComponent()
                .AddColumn("Nome", 3)
                .AddColumn("Status", 2)
                .AddColumn("Tipo", 2)
                .AddColumn("Valor", 2);
                
            foreach (var pendencia in pendencias)
            {
                tabela.AddRow(
                    pendencia.Nome ?? "Sem nome",
                    pendencia.Status.ToString(),
                    pendencia.TipoDivida.ToString(),
                    $"{pendencia.ValorTotal:C2}"
                );
            }
            
            return tabela;
        }
    }
}
