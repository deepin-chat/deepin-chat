using MassTransit;

namespace Deepin.Infrastructure.EventBus;
public interface IIntegrationEventConsumer<in TIntegrationEvent> : IConsumer<TIntegrationEvent>
     where TIntegrationEvent : IntegrationEvent
{
}