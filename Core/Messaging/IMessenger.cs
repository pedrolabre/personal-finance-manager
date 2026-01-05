using System;

namespace PersonalFinanceManager.Core.Messaging;

public interface IMessenger
{
    void Send<TMessage>(TMessage message) where TMessage : class;
    void Register<TMessage>(object recipient, Action<TMessage> action) where TMessage : class;
    void Unregister(object recipient);
    void Unregister<TMessage>(object recipient) where TMessage : class;
}
