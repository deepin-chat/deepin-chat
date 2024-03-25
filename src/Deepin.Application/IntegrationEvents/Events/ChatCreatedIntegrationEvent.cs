using Deepin.Infrastructure.EventBus;

namespace Deepin.Application.IntegrationEvents.Events;

public record ChatCreatedIntegrationEvent : IntegrationEvent
{
    public ChatCreatedIntegrationEvent(string userId)
    {
        UserId = userId;
    }
    public string UserId { get; set; }
}