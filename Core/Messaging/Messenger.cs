using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalFinanceManager.Core.Messaging;

public class Messenger : IMessenger
{
    private readonly Dictionary<Type, List<Subscription>> _subscriptions = new();
    private readonly object _lock = new();



    public void Send<TMessage>(TMessage message) where TMessage : class
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        List<Subscription> subscriptions;

        lock (_lock)
        {
            var messageType = typeof(TMessage);
            if (!_subscriptions.TryGetValue(messageType, out subscriptions))
                return;

            subscriptions = subscriptions.ToList();
        }

        foreach (var subscription in subscriptions)
        {
            if (subscription.Recipient.TryGetTarget(out var target))
            {
                subscription.Action.DynamicInvoke(message);
            }
        }
    }

    public void Register<TMessage>(object recipient, Action<TMessage> action) where TMessage : class
    {
        if (recipient == null)
            throw new ArgumentNullException(nameof(recipient));
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        lock (_lock)
        {
            var messageType = typeof(TMessage);

            if (!_subscriptions.TryGetValue(messageType, out var subscriptions))
            {
                subscriptions = new List<Subscription>();
                _subscriptions[messageType] = subscriptions;
            }

            subscriptions.Add(new Subscription(recipient, action));
        }
    }

    public void Unregister(object recipient)
    {
        if (recipient == null)
            throw new ArgumentNullException(nameof(recipient));
        
        lock (_lock)
        {
            foreach (var subscriptions in _subscriptions.Values)
            {
                subscriptions.RemoveAll(s => 
                    !s.Recipient.TryGetTarget(out var target) || target == recipient);
            }
        }
    }
    
    public void Unregister<TMessage>(object recipient) where TMessage : class
    {
        if (recipient == null)
            throw new ArgumentNullException(nameof(recipient));
        
        lock (_lock)
        {
            var messageType = typeof(TMessage);
            
            if (_subscriptions.TryGetValue(messageType, out var subscriptions))
            {
                subscriptions.RemoveAll(s => 
                    !s.Recipient.TryGetTarget(out var target) || target == recipient);
            }
        }
    }
    
    private class Subscription
    {
        public WeakReference<object> Recipient { get; }
        public Delegate Action { get; }

        public Subscription(object recipient, Delegate action)
        {
            Recipient = new WeakReference<object>(recipient);
            Action = action;
        }
    }
}
