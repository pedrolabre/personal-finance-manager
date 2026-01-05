using System;
// ...existing code...
using PersonalFinanceManager.Services.Notifications.Models;
using System.Threading.Tasks;
#if WINDOWS
// ...existing code...
// using Windows.UI.Notifications; // Removido pois não existe em .NET 10 WPF
using System.Collections.Generic;
// ...existing code...
// ...existing code...
#endif

namespace PersonalFinanceManager.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationScheduler _scheduler;
        private NotificationConfig _config;

        public NotificationService(INotificationScheduler scheduler)
        {
            _scheduler = scheduler;
            _config = NotificationConfig.Default;
        }

        public void MostrarNotificacao(NotificationMessage message)
        {
            if (!_config.NotificacoesAtivadas)
                return;

            // Usar Dispatcher para garantir execução na thread UI
            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                // Determinar ícone baseado no tipo de notificação
                var icon = message.Tipo switch
                {
                    NotificationType.Vencimento => System.Windows.MessageBoxImage.Warning,
                    NotificationType.DividaAtrasada => System.Windows.MessageBoxImage.Error,
                    NotificationType.Alerta => System.Windows.MessageBoxImage.Exclamation,
                    _ => System.Windows.MessageBoxImage.Information
                };

                System.Windows.MessageBox.Show(
                    message.Mensagem,
                    message.Titulo,
                    System.Windows.MessageBoxButton.OK,
                    icon
                );
            });
        }

        public void AgendarNotificacaoVencimento(int parcelaId, DateTime dataVencimento)
        {
            if (!_config.NotificarVencimentos)
                return;

            // Notificar X dias antes (configurável)
            var dataNotificacao = dataVencimento.AddDays(-_config.DiasAntecipadosVencimento);

            if (dataNotificacao <= DateTime.Now)
                return; // Já passou

            var message = new NotificationMessage
            {
                Id = Guid.NewGuid().ToString(),
                ReferenceId = parcelaId,
                Titulo = "Vencimento Próximo",
                Mensagem = $"Parcela vence em {_config.DiasAntecipadosVencimento} dias",
                DataAgendamento = dataNotificacao,
                Tipo = NotificationType.Vencimento
            };

            _scheduler.Agendar(message);
        }

        public void CancelarNotificacao(int notificationId)
        {
            _scheduler.Cancelar(notificationId);
        }

        public async Task<IEnumerable<NotificationMessage>> ObterNotificacoesPendentesAsync()
        {
            return await _scheduler.ObterPendentesAsync();
        }

        public void ConfigurarNotificacoes(NotificationConfig config)
        {
            _config = config;
            // Salvar configurações no banco ou JSON local
        }
    }
}
