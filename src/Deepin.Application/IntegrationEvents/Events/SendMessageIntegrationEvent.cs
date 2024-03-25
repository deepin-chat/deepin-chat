using Deepin.Application.Models.Messages;
using Deepin.Infrastructure.EventBus;

namespace Deepin.Application.IntegrationEvents.Events;
public record SendMessageIntegrationEvent : IntegrationEvent
{
    public MessageModel Message { get; set; }
    public string UserId { get; set; }
    public SendMessageIntegrationEvent(string userId, MessageModel message)
    {
        Message = message;
        UserId = userId;
    }
}
