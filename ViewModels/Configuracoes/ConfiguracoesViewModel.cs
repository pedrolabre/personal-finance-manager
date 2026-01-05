using System;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using PersonalFinanceManager.ViewModels.Base;
using PersonalFinanceManager.Core.Commands;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;
using PersonalFinanceManager.Models;

namespace PersonalFinanceManager.ViewModels.Configuracoes
{
    public class ConfiguracoesViewModel : ViewModelBase
    {
        private readonly IMessenger _messenger;
        private readonly string _settingsFilePath;
        private string _caminhoPadraoRelatorios = string.Empty;
        private bool _exibirGraficosRelatorios = true;
        private bool _incluirDetalhesRelatorios = true;
        private string _versaoAplicacao = string.Empty;
        private string _caminhoBaseDados = string.Empty;

        public string CaminhoPadraoRelatorios
        {
            get => _caminhoPadraoRelatorios;
            set => SetProperty(ref _caminhoPadraoRelatorios, value);
        }

        public bool ExibirGraficosRelatorios
        {
            get => _exibirGraficosRelatorios;
            set => SetProperty(ref _exibirGraficosRelatorios, value);
        }

        public bool IncluirDetalhesRelatorios
        {
            get => _incluirDetalhesRelatorios;
            set => SetProperty(ref _incluirDetalhesRelatorios, value);
        }

        public string VersaoAplicacao
        {
            get => _versaoAplicacao;
            set => SetProperty(ref _versaoAplicacao, value);
        }

        public string CaminhoBaseDados
        {
            get => _caminhoBaseDados;
            set => SetProperty(ref _caminhoBaseDados, value);
        }

        public ICommand SalvarCommand { get; }
        public ICommand AbrirPastaRelatoriosCommand { get; }
        public ICommand AbrirPastaBaseDadosCommand { get; }
        public ICommand RestaurarPadroesCommand { get; }

        public ConfiguracoesViewModel(IMessenger messenger)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

            // Caminho do arquivo de configurações
            _settingsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PersonalFinanceManager",
                "appsettings.json");

            // Carregar configurações salvas ou usar valores padrão
            CarregarConfiguracoes();

            CaminhoBaseDados = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PersonalFinanceManager",
                "finance.db");

            VersaoAplicacao = "1.19.7";

            SalvarCommand = new RelayCommand(ExecuteSalvar, () => true);
            AbrirPastaRelatoriosCommand = new RelayCommand(ExecuteAbrirPastaRelatorios);
            AbrirPastaBaseDadosCommand = new RelayCommand(ExecuteAbrirPastaBaseDados);
            RestaurarPadroesCommand = new RelayCommand(ExecuteRestaurarPadroes);
        }

        private void ExecuteSalvar()
        {
            try
            {
                var settings = new AppSettings
                {
                    CaminhoPadraoRelatorios = CaminhoPadraoRelatorios,
                    ExibirGraficosRelatorios = ExibirGraficosRelatorios,
                    IncluirDetalhesRelatorios = IncluirDetalhesRelatorios
                };

                // Garantir que o diretório existe
                var directory = Path.GetDirectoryName(_settingsFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Salvar no arquivo JSON
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(_settingsFilePath, json);

                _messenger.Send(new SuccessMessage("Configurações salvas com sucesso!"));
            }
            catch (Exception ex)
            {
                _messenger.Send(new ErrorMessage("Erro ao salvar configurações", ex));
            }
        }

        private void CarregarConfiguracoes()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);

                    if (settings != null)
                    {
                        CaminhoPadraoRelatorios = settings.CaminhoPadraoRelatorios;
                        ExibirGraficosRelatorios = settings.ExibirGraficosRelatorios;
                        IncluirDetalhesRelatorios = settings.IncluirDetalhesRelatorios;
                        return;
                    }
                }

                // Se não existe arquivo ou falhou ao carregar, usar valores padrão
                CaminhoPadraoRelatorios = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "FinanceManager",
                    "Reports");
                ExibirGraficosRelatorios = true;
                IncluirDetalhesRelatorios = true;
            }
            catch
            {
                // Em caso de erro, usar valores padrão
                CaminhoPadraoRelatorios = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "FinanceManager",
                    "Reports");
                ExibirGraficosRelatorios = true;
                IncluirDetalhesRelatorios = true;
            }
        }

        private void ExecuteAbrirPastaRelatorios()
        {
            try
            {
                if (!Directory.Exists(CaminhoPadraoRelatorios))
                {
                    Directory.CreateDirectory(CaminhoPadraoRelatorios);
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = CaminhoPadraoRelatorios,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _messenger.Send(new ErrorMessage("Erro ao abrir pasta de relatórios", ex));
            }
        }

        private void ExecuteAbrirPastaBaseDados()
        {
            try
            {
                var pasta = Path.GetDirectoryName(CaminhoBaseDados);
                if (!string.IsNullOrEmpty(pasta) && Directory.Exists(pasta))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = pasta,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                _messenger.Send(new ErrorMessage("Erro ao abrir pasta de base de dados", ex));
            }
        }

        private void ExecuteRestaurarPadroes()
        {
            ExibirGraficosRelatorios = true;
            IncluirDetalhesRelatorios = true;
            CaminhoPadraoRelatorios = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "FinanceManager",
                "Reports");

            // Salvar os padrões no arquivo
            ExecuteSalvar();

            _messenger.Send(new InfoMessage("Configurações restauradas para os valores padrão"));
        }
    }
}
