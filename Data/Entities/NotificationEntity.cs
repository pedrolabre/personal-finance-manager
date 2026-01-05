using System;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Data.Entities
{
    public class NotificationEntity
    {
        public int Id { get; set; }
        public string NotificationId { get; set; } // GUID
        public int ReferenceId { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public DateTime DataAgendamento { get; set; }
        public NotificationType Tipo { get; set; }
        public bool Enviada { get; set; }
        public DateTime? DataEnvio { get; set; }
        public bool Cancelada { get; set; }
    }
}
