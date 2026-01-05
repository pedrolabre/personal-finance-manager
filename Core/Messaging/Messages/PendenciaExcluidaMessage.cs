namespace PersonalFinanceManager.Core.Messaging.Messages;

public class PendenciaExcluidaMessage
{
    public int PendenciaId { get; }
    
    public PendenciaExcluidaMessage(int pendenciaId)
    {
        PendenciaId = pendenciaId;
    }
}
