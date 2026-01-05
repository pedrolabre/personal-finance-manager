using System;
using System.Collections.Generic;

namespace PersonalFinanceManager.TempModels;

public partial class Notification
{
    public int Id { get; set; }

    public string NotificationId { get; set; }

    public int ReferenceId { get; set; }

    public string Titulo { get; set; }

    public string Mensagem { get; set; }

    public string DataAgendamento { get; set; }

    public int Tipo { get; set; }

    public int Enviada { get; set; }

    public string DataEnvio { get; set; }

    public int Cancelada { get; set; }
}
