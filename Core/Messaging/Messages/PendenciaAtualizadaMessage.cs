namespace PersonalFinanceManager.Core.Messaging.Messages;

public class PendenciaAtualizadaMessage
{
    public int PendenciaId { get; }
    
    public PendenciaAtualizadaMessage(int pendenciaId)
    {
        PendenciaId = pendenciaId;
    }
}
