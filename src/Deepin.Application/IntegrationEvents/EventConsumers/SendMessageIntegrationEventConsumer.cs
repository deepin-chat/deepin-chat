using Deepin.Application.Constants;
using Deepin.Application.Hubs;
using Deepin.Application.IntegrationEvents.Events;
using Deepin.Infrastructure.EventBus;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Deepin.Application.IntegrationEvents.EventConsumers;
public class SendMessageIntegrationEventConsumer(IHubContext<ChatHub> chatHub) : IIntegrationEventConsumer<SendMessageIntegrationEvent>
{
    private readonly IHubContext<ChatHub> _chatHub = chatHub;
    public async Task Consume(ConsumeContext<SendMessageIntegrationEvent> context)
    {
       await _chatHub.Clients.Group(context.Message.Message.ChatId.ToString()).SendAsync(HubMethodNames.NewMessage, context.Message.Message);
    }
}
