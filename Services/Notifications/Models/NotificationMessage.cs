using System;

namespace PersonalFinanceManager.Services.Notifications.Models
{
    public class NotificationMessage
    {
        public string Id { get; set; }
        public int ReferenceId { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public DateTime DataAgendamento { get; set; }
        public NotificationType Tipo { get; set; }
        public bool Enviada { get; set; }
    }

    public enum NotificationType
    {
        Vencimento,
        Recebimento,
        DividaAtrasada,
        Alerta
    }
}
