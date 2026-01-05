using System;
namespace PersonalFinanceManager.Core.Messaging.Messages;

public class WarningMessage
{
    public string Message { get; }

    public WarningMessage(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}
