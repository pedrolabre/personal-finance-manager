
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceManager.Core.Messaging.Messages;
using System.Windows.Input;
using PersonalFinanceManager.Core.Messaging;
using PersonalFinanceManager.Core.Commands;
using PersonalFinanceManager.Services.Import;
using PersonalFinanceManager.Services.Import.Models;
using PersonalFinanceManager.ViewModels.Base;

namespace PersonalFinanceManager.ViewModels.Import
{
    public class TextImportViewModel : ViewModelBase
    {
        private readonly IImportService _importService;
        private readonly IMessenger _messenger;
        private string _textoImportacao;
        private ImportFormat _formatoSelecionado;
        private ObservableCollection<ImportedPendencia> _pendenciasPreview;
        private bool _mostrarPreview;
        private bool _podeImportar;

        public ObservableCollection<ImportFormat> FormatosDisponiveis { get; }

        public string TextoImportacao
        {
            get => _textoImportacao;
            set
            {
                SetProperty(ref _textoImportacao, value);
                PodeImportar = false;
            }
        }

        public ImportFormat FormatoSelecionado
        {
            get => _formatoSelecionado;
            set => SetProperty(ref _formatoSelecionado, value);
        }

        public ObservableCollection<ImportedPendencia> PendenciasPreview
        {
            get => _pendenciasPreview;
            set => SetProperty(ref _pendenciasPreview, value);
        }

        public bool MostrarPreview
        {
            get => _mostrarPreview;
            set => SetProperty(ref _mostrarPreview, value);
        }

        public bool PodeImportar
        {
            get => _podeImportar;
            set => SetProperty(ref _podeImportar, value);
        }

        public ICommand ValidarCommand { get; }
        public ICommand ImportarCommand { get; }
        public ICommand MostrarExemploCommand { get; }

        public TextImportViewModel(IImportService importService, IMessenger messenger)
        {
            _importService = importService;
            _messenger = messenger;

            FormatosDisponiveis = new ObservableCollection<ImportFormat>
            {
                ImportFormat.FormatoSimples,
                ImportFormat.FormatoCsv,
                ImportFormat.FormatoJson
            };
            FormatoSelecionado = FormatosDisponiveis[0];

            ValidarCommand = new AsyncRelayCommand(ExecuteValidarAsync);
            ImportarCommand = new AsyncRelayCommand(ExecuteImportarAsync);
            MostrarExemploCommand = new RelayCommand(ExecuteMostrarExemplo);
        }

        private async Task ExecuteValidarAsync()
        {
            if (string.IsNullOrWhiteSpace(TextoImportacao))
            {
                _messenger.Send(new ErrorMessage("Digite algo para validar"));
                return;
            }

            var result = await _importService.ValidarTextoAsync(TextoImportacao, FormatoSelecionado);

            if (result.Sucesso)
            {
                PendenciasPreview = new ObservableCollection<ImportedPendencia>(result.Pendencias);
                MostrarPreview = true;
                PodeImportar = result.Pendencias.Any();

                if (result.Avisos.Any())
                    _messenger.Send(new WarningMessage($"Validado com avisos:\n{string.Join("\n", result.Avisos)}"));
                else
                    _messenger.Send(new SuccessMessage($"{result.TotalRegistros} registros válidos"));
            }
            else
            {
                _messenger.Send(new ErrorMessage($"Erros:\n{string.Join("\n", result.Erros)}"));
            }
        }

        private async Task ExecuteImportarAsync()
        {
            var result = await _importService.ImportarDeTextoAsync(TextoImportacao, FormatoSelecionado);

            if (result.Sucesso)
            {
                _messenger.Send(new SuccessMessage(result.ResumoImportacao));
                // Corrigir: passar a lista de pendências importadas
                _messenger.Send(new PendenciasImportadasMessage(result.Pendencias));

                // Limpar formulário
                TextoImportacao = string.Empty;
                PendenciasPreview = null;
                MostrarPreview = false;
                PodeImportar = false;
            }
            else
            {
                _messenger.Send(new ErrorMessage($"Falha na importação:\n{string.Join("\n", result.Erros)}"));
            }
        }

        private void ExecuteMostrarExemplo()
        {
            TextoImportacao = FormatoSelecionado.Exemplo;
        }
    }
}
