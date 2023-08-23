
namespace PrismFeaturesReplacement.Services;

public interface IEventAggregator
{
    void SubscribeEvent<TEventType>(object recipient, MessageHandler<object, TEventType> handler) where TEventType : class;

    void UnsubscribeEvent<TEventType>(object recipient) where TEventType : class;

    TEventType PublishEvent<TEventType>(TEventType @event) where TEventType : class;
}

