#nullable enable
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using PersonalFinanceManager.Core.Commands;
using PersonalFinanceManager.Models;
using PersonalFinanceManager.ViewModels.Base;
using PersonalFinanceManager.Services.Reports.Models;

namespace PersonalFinanceManager.ViewModels.Relatorios
{
    public class RelatoriosViewModel : ViewModelBase
    {
        private DateTime? _dataInicio;
        public DateTime? DataInicio
        {
            get => _dataInicio;
            set => SetProperty(ref _dataInicio, value);
        }

        private DateTime? _dataFim;
        public DateTime? DataFim
        {
            get => _dataFim;
            set => SetProperty(ref _dataFim, value);
        }

        private RelayCommand? _abrirPastaRelatorioCommandImpl;
        private RelayCommand? _abrirArquivoRelatorioCommandImpl;
        
        private string? _ultimoCaminhoRelatorio;
        public string? UltimoCaminhoRelatorio
        {
            get => _ultimoCaminhoRelatorio;
            set
            {
                if (SetProperty(ref _ultimoCaminhoRelatorio, value))
                {
                    _abrirPastaRelatorioCommandImpl?.RaiseCanExecuteChanged();
                    _abrirArquivoRelatorioCommandImpl?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand AbrirPastaRelatorioCommand { get; }
        public ICommand AbrirArquivoRelatorioCommand { get; }

        private string? _relatorioVisual;

        public ObservableCollection<TipoRelatorioItem> TiposRelatorio { get; }

        public TipoRelatorioItem? TipoRelatorioSelecionado
        {
            get => _tipoRelatorioSelecionadoItem;
            set => SetProperty(ref _tipoRelatorioSelecionadoItem, value);
        }
        private TipoRelatorioItem? _tipoRelatorioSelecionadoItem;

        public ICommand GerarRelatorioCommand { get; }

        public string? RelatorioVisual
        {
            get => _relatorioVisual;
            set => SetProperty(ref _relatorioVisual, value);
        }

        private readonly PersonalFinanceManager.Services.Reports.IReportService _reportService;
        public RelatoriosViewModel(PersonalFinanceManager.Services.Reports.IReportService reportService)
        {
            _reportService = reportService;
            TiposRelatorio = new ObservableCollection<TipoRelatorioItem>(
                Enum.GetValues(typeof(TipoRelatorio)).Cast<TipoRelatorio>()
                .Select(e => new TipoRelatorioItem(e)));
            TipoRelatorioSelecionado = TiposRelatorio.FirstOrDefault();
            GerarRelatorioCommand = new RelayCommand(ExecuteGerarRelatorio);
            
            _abrirPastaRelatorioCommandImpl = new RelayCommand(
                ExecuteAbrirPastaRelatorio, 
                () => !string.IsNullOrEmpty(UltimoCaminhoRelatorio));
            AbrirPastaRelatorioCommand = _abrirPastaRelatorioCommandImpl;
            
            _abrirArquivoRelatorioCommandImpl = new RelayCommand(
                ExecuteAbrirArquivoRelatorio, 
                () => !string.IsNullOrEmpty(UltimoCaminhoRelatorio) && System.IO.File.Exists(UltimoCaminhoRelatorio));
            AbrirArquivoRelatorioCommand = _abrirArquivoRelatorioCommandImpl;
        }

        private async void ExecuteGerarRelatorio()
        {
            try
            {
                System.IO.File.AppendAllText("log_geracao_relatorio.txt", $"[CALL] ExecuteGerarRelatorio chamado: {System.DateTime.Now}\n");
                
                // Carregar configurações salvas
                var settings = CarregarConfiguracoes();
                
                var options = new PersonalFinanceManager.Services.Reports.Models.ReportOptions
                {
                    TipoRelatorio = TipoRelatorioSelecionado?.Tipo ?? PersonalFinanceManager.Services.Reports.Models.TipoRelatorio.TodasPendencias,
                    DataInicio = DataInicio,
                    DataFim = DataFim,
                    OutputDirectory = !string.IsNullOrEmpty(settings.CaminhoPadraoRelatorios) ? settings.CaminhoPadraoRelatorios : null,
                    IncluirGraficos = settings.ExibirGraficosRelatorios,
                    IncluirDetalhes = settings.IncluirDetalhesRelatorios
                };

                string? path = null;
                
                switch (options.TipoRelatorio)
                {
                    case PersonalFinanceManager.Services.Reports.Models.TipoRelatorio.TodasPendencias:
                        path = await _reportService.GerarRelatorioPendenciasAsync(options);
                        break;
                    case PersonalFinanceManager.Services.Reports.Models.TipoRelatorio.PendenciasAtrasadas:
                        options.FiltrarPorStatus = PersonalFinanceManager.Models.Enums.StatusPendencia.Atrasada;
                        path = await _reportService.GerarRelatorioPendenciasAsync(options);
                        break;
                    case PersonalFinanceManager.Services.Reports.Models.TipoRelatorio.ResumoCartoes:
                        path = await _reportService.GerarRelatorioCartoesAsync(options);
                        break;
                    case PersonalFinanceManager.Services.Reports.Models.TipoRelatorio.DashboardCompleto:
                        path = await _reportService.GerarRelatorioDashboardAsync();
                        break;
                }
                UltimoCaminhoRelatorio = path;
                RelatorioVisual = $"Relatório gerado em: {path}";
            }
            catch (Exception ex)
            {
                RelatorioVisual = $"Erro ao gerar relatório: {ex.Message}";
            }
        }

        private void ExecuteAbrirPastaRelatorio()
        {
            if (!string.IsNullOrEmpty(UltimoCaminhoRelatorio))
            {
                var pasta = System.IO.Path.GetDirectoryName(UltimoCaminhoRelatorio);
                if (!string.IsNullOrEmpty(pasta) && System.IO.Directory.Exists(pasta))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = pasta,
                        UseShellExecute = true
                    });
                }
            }
        }

        private void ExecuteAbrirArquivoRelatorio()
        {
            if (!string.IsNullOrEmpty(UltimoCaminhoRelatorio) && System.IO.File.Exists(UltimoCaminhoRelatorio))
            {
                // Abre o arquivo PDF diretamente no visualizador padrão
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = UltimoCaminhoRelatorio,
                    UseShellExecute = true
                });
            }
        }

        private AppSettings CarregarConfiguracoes()
        {
            try
            {
                var settingsFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "PersonalFinanceManager",
                    "appsettings.json");

                if (File.Exists(settingsFilePath))
                {
                    var json = File.ReadAllText(settingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    if (settings != null)
                    {
                        return settings;
                    }
                }
            }
            catch
            {
                // Em caso de erro, retorna configurações padrão
            }

            // Retornar valores padrão
            return new AppSettings
            {
                CaminhoPadraoRelatorios = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "FinanceManager",
                    "Reports"),
                ExibirGraficosRelatorios = true,
                IncluirDetalhesRelatorios = true
            };
        }
    }

    public class TipoRelatorioItem
    {
        public TipoRelatorio Tipo { get; }
        public string NomeAmigavel { get; }
        public TipoRelatorioItem(TipoRelatorio tipo)
        {
            Tipo = tipo;
            NomeAmigavel = tipo switch
            {
                TipoRelatorio.TodasPendencias => "Todas as Pendências",
                TipoRelatorio.PendenciasAtrasadas => "Pendências Atrasadas",
                TipoRelatorio.ResumoCartoes => "Resumo de Cartões",
                TipoRelatorio.DashboardCompleto => "Dashboard Completo",
                _ => tipo.ToString()
            };
        }
        public override string ToString() => NomeAmigavel;
    }
}
