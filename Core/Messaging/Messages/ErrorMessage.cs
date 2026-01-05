#nullable enable
using System;
namespace PersonalFinanceManager.Core.Messaging.Messages;

public class ErrorMessage
{
    public string Message { get; }
    public Exception? Exception { get; }
    
    public ErrorMessage(string message, Exception? exception = null)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Exception = exception;
    }
}

public class InfoMessage
{
    public string Message { get; }
    
    public InfoMessage(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}
