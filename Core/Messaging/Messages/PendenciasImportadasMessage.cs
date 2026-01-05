using System;
using System.Collections.Generic;
using PersonalFinanceManager.Services.Import.Models;
namespace PersonalFinanceManager.Core.Messaging.Messages;
public class PendenciasImportadasMessage
{
    public IEnumerable<ImportedPendencia> Pendencias { get; }
    public PendenciasImportadasMessage(IEnumerable<ImportedPendencia> pendencias)
    {
        Pendencias = pendencias ?? throw new ArgumentNullException(nameof(pendencias));
    }
}