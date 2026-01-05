using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PersonalFinanceManager.Services.Notifications.Models;

namespace PersonalFinanceManager.Services.Notifications
{
    public interface INotificationScheduler : IDisposable
    {
        void Agendar(NotificationMessage message);
        void Cancelar(int notificationId);
        Task<IEnumerable<NotificationMessage>> ObterPendentesAsync();
    }

    public interface INotificationRepository
    {
        void SalvarNotificacao(NotificationMessage message);
        Task<IEnumerable<NotificationMessage>> ObterNotificacoesPendentesAsync();
        Task MarcarComoEnviadaAsync(string notificationId);
    }

    public class NotificationScheduler : INotificationScheduler
    {
        private readonly Timer _timer;
        private readonly INotificationRepository _repository;
        private readonly INotificationService _notificationService;

        public NotificationScheduler(
            INotificationRepository repository,
            INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;

            // Verificar a cada hora
            _timer = new Timer(VerificarNotificacoesPendentes, null,
                TimeSpan.Zero, TimeSpan.FromHours(1));
        }

        public void Agendar(NotificationMessage message)
        {
            _repository.SalvarNotificacao(message);
        }

        public void Cancelar(int notificationId)
        {
            // Implementação para cancelar notificação agendada
        }

        public async Task<IEnumerable<NotificationMessage>> ObterPendentesAsync()
        {
            return await _repository.ObterNotificacoesPendentesAsync();
        }

        private async void VerificarNotificacoesPendentes(object state)
        {
            var pendentes = await _repository.ObterNotificacoesPendentesAsync();

            foreach (var notificacao in pendentes)
            {
                if (notificacao.DataAgendamento <= DateTime.Now)
                {
                    _notificationService.MostrarNotificacao(notificacao);
                    await _repository.MarcarComoEnviadaAsync(notificacao.Id);
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
