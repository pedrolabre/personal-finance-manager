using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalFinanceManager.Services.Notifications.Models;

namespace PersonalFinanceManager.Services.Notifications
{
    public interface INotificationService
    {
        void MostrarNotificacao(NotificationMessage message);
        void AgendarNotificacaoVencimento(int parcelaId, DateTime dataVencimento);
        void CancelarNotificacao(int notificationId);
        Task<IEnumerable<NotificationMessage>> ObterNotificacoesPendentesAsync();
        void ConfigurarNotificacoes(NotificationConfig config);
    }
}
