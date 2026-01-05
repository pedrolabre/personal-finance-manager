using System;
namespace PersonalFinanceManager.Core.Messaging.Messages;

public class SuccessMessage
{
    public string Message { get; }

    public SuccessMessage(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}
