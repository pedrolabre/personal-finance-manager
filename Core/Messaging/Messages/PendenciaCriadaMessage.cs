namespace PersonalFinanceManager.Core.Messaging.Messages;

public class PendenciaCriadaMessage
{
    public int PendenciaId { get; }
    
    public PendenciaCriadaMessage(int pendenciaId)
    {
        PendenciaId = pendenciaId;
    }
}
