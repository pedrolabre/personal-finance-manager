using System;

namespace PersonalFinanceManager.Services.Notifications.Models
{
    public class NotificationConfig
    {
        public bool NotificacoesAtivadas { get; set; }
        public bool NotificarVencimentos { get; set; }
        public bool NotificarRecebimentos { get; set; }
        public bool NotificarDividasAtrasadas { get; set; }
        public int DiasAntecipadosVencimento { get; set; }
        public TimeSpan HorarioNotificacao { get; set; }
        public bool NotificarFinaisDeSemana { get; set; }

        public static NotificationConfig Default => new()
        {
            NotificacoesAtivadas = true,
            NotificarVencimentos = true,
            NotificarRecebimentos = true,
            NotificarDividasAtrasadas = true,
            DiasAntecipadosVencimento = 3,
            HorarioNotificacao = new TimeSpan(9, 0, 0), // 9h da manhã
            NotificarFinaisDeSemana = false
        };
    }
}
