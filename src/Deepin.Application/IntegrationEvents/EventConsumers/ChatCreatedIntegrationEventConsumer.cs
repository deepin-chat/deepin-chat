using Deepin.Application.Constants;
using Deepin.Application.Hubs;
using Deepin.Application.IntegrationEvents.Events;
using Deepin.Infrastructure.EventBus;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Deepin.Application.IntegrationEvents.EventConsumers;

public class ChatCreatedIntegrationEventConsumer(IHubContext<ChatHub> chatHub) : IIntegrationEventConsumer<ChatCreatedIntegrationEvent>
{
    private readonly IHubContext<ChatHub> _chatHub = chatHub;
    public async Task Consume(ConsumeContext<ChatCreatedIntegrationEvent> context)
    {
        await _chatHub.Clients.Users(context.Message.UserId).SendAsync(HubMethodNames.NewChat);
    }
}

