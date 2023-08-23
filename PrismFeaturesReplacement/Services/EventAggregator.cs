namespace PrismFeaturesReplacement.Services;

public class EventAggregator : IEventAggregator
{
    public void SubscribeEvent<TEventType>(object recipient, MessageHandler<object, TEventType> handler) where TEventType : class
    {
        WeakReferenceMessenger.Default.Register(recipient, handler);
    }

    public void UnsubscribeEvent<TEventType>(object recipient) where TEventType : class
    {
        WeakReferenceMessenger.Default.Unregister<TEventType>(recipient);
    }

    public TEventType PublishEvent<TEventType>(TEventType @event) where TEventType : class
    {
        return WeakReferenceMessenger.Default.Send(@event);
    }
}

