#nullable enable
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using PersonalFinanceManager.Core.Commands;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Messaging.Messages;
using PersonalFinanceManager.Core.Navigation;
using PersonalFinanceManager.Services.Import;
using PersonalFinanceManager.Services.Import.Models;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager.ViewModels.Importacao
{
    public class ImportacaoViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IMessenger _messenger;
        private readonly IImportService _importService;
        
        private AsyncRelayCommand? _importarCommandImpl;
        
        private string _caminhoArquivo = string.Empty;
        private bool _podeImportar;
        private bool _isLoading;
        private string _statusImportacao = string.Empty;
        private int _totalRegistros;
        private int _registrosValidos;
        
        public string CaminhoArquivo
        {
            get => _caminhoArquivo;
            set
            {
                if (SetProperty(ref _caminhoArquivo, value))
                {
                    PodeImportar = !string.IsNullOrEmpty(value) && File.Exists(value);
                    if (PodeImportar)
                    {
                        _ = CarregarPreviewAsync();
                    }
                    _importarCommandImpl?.RaiseCanExecuteChanged();
                }
            }
        }
        
        public bool PodeImportar
        {
            get => _podeImportar;
            set
            {
                if (SetProperty(ref _podeImportar, value))
                {
                    _importarCommandImpl?.RaiseCanExecuteChanged();
                }
            }
        }
        
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    _importarCommandImpl?.RaiseCanExecuteChanged();
                }
            }
        }
        
        public string StatusImportacao
        {
            get => _statusImportacao;
            set => SetProperty(ref _statusImportacao, value);
        }
        
        public int TotalRegistros
        {
            get => _totalRegistros;
            set => SetProperty(ref _totalRegistros, value);
        }
        
        public int RegistrosValidos
        {
            get => _registrosValidos;
            set => SetProperty(ref _registrosValidos, value);
        }
        
        public ObservableCollection<string[]> PreviewLinhas { get; } = new();
        public ObservableCollection<string> Erros { get; } = new();
        public ObservableCollection<string> Avisos { get; } = new();
        
        public ICommand ProcurarArquivoCommand { get; }
        public ICommand ImportarCommand { get; }
        public ICommand CancelarCommand { get; }
        
        public ImportacaoViewModel(
            INavigationService navigationService, 
            IMessenger messenger,
            IImportService importService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            _importService = importService ?? throw new ArgumentNullException(nameof(importService));
            
            ProcurarArquivoCommand = new RelayCommand(ExecuteProcurarArquivo);
            _importarCommandImpl = new AsyncRelayCommand(ExecuteImportarAsync, () => PodeImportar && !IsLoading);
            ImportarCommand = _importarCommandImpl;
            CancelarCommand = new RelayCommand(ExecuteCancelar);
        }
        
        private void ExecuteProcurarArquivo()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Selecione o arquivo para importar",
                Filter = "Arquivos CSV (*.csv)|*.csv|Arquivos de Texto (*.txt)|*.txt|Todos os arquivos (*.*)|*.*",
                FilterIndex = 1
            };
            
            if (dialog.ShowDialog() == true)
            {
                CaminhoArquivo = dialog.FileName;
            }
        }
        
        private async Task CarregarPreviewAsync()
        {
            PreviewLinhas.Clear();
            Erros.Clear();
            Avisos.Clear();
            StatusImportacao = "Analisando arquivo...";
            
            try
            {
                var conteudo = await File.ReadAllTextAsync(CaminhoArquivo);
                
                // Validar com o serviço de importação
                var resultado = await _importService.ValidarTextoAsync(conteudo, ImportFormat.FormatoCsv);
                
                TotalRegistros = resultado.TotalRegistros;
                RegistrosValidos = resultado.Pendencias?.Count ?? 0;
                
                // Mostrar preview das primeiras 10 linhas do arquivo
                var linhas = conteudo.Split('\n')
                    .Take(10)
                    .Select(l => l.Split(';', ',', '\t'))
                    .ToList();
                
                foreach (var linha in linhas)
                {
                    PreviewLinhas.Add(linha);
                }
                
                // Mostrar erros e avisos
                foreach (var erro in resultado.Erros)
                {
                    Erros.Add(erro);
                }
                
                foreach (var aviso in resultado.Avisos)
                {
                    Avisos.Add(aviso);
                }
                
                if (resultado.Sucesso)
                {
                    StatusImportacao = $"✓ {RegistrosValidos} registros válidos de {TotalRegistros} encontrados";
                    PodeImportar = RegistrosValidos > 0;
                }
                else
                {
                    StatusImportacao = $"⚠ Problemas encontrados: {Erros.Count} erros, {Avisos.Count} avisos";
                    PodeImportar = RegistrosValidos > 0;
                }
            }
            catch (Exception ex)
            {
                StatusImportacao = $"✗ Erro ao analisar arquivo: {ex.Message}";
                PodeImportar = false;
                _messenger.Send(new ErrorMessage("Erro ao carregar preview do arquivo", ex));
            }
        }
        
        private async Task ExecuteImportarAsync()
        {
            IsLoading = true;
            StatusImportacao = "Importando...";
            Erros.Clear();
            
            try
            {
                var conteudo = await File.ReadAllTextAsync(CaminhoArquivo);
                var resultado = await _importService.ImportarDeTextoAsync(conteudo, ImportFormat.FormatoCsv);
                
                if (resultado.Sucesso)
                {
                    var mensagem = $"Importação concluída!\n" +
                                   $"• Registros importados: {resultado.RegistrosImportados}\n" +
                                   $"• Falhas: {resultado.RegistrosFalhos}";
                    
                    if (resultado.Avisos.Any())
                    {
                        mensagem += $"\n• Avisos: {resultado.Avisos.Count}";
                    }
                    
                    _messenger.Send(new SuccessMessage(mensagem));
                    _navigationService.NavigateTo<DashboardViewModel>();
                }
                else
                {
                    foreach (var erro in resultado.Erros)
                    {
                        Erros.Add(erro);
                    }
                    
                    StatusImportacao = $"✗ Falha na importação: {resultado.Erros.FirstOrDefault() ?? "Erro desconhecido"}";
                    _messenger.Send(new ErrorMessage($"Falha ao importar: {resultado.Erros.FirstOrDefault()}"));
                }
            }
            catch (Exception ex)
            {
                StatusImportacao = $"✗ Erro: {ex.Message}";
                _messenger.Send(new ErrorMessage("Erro ao importar arquivo", ex));
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private void ExecuteCancelar()
        {
            _navigationService.NavigateTo<DashboardViewModel>();
        }
    }
}
