using Deepin.Domain.Entities;
using MediatR;

namespace Deepin.Domain.Events;
public record ChatMemberJoinedDomainEvent : INotification
{
    public Chat Chat { get; set; }
    public string UserId { get; set; }
    public ChatMemberJoinedDomainEvent(Chat chat, string userId)
    {
        Chat = chat;
        UserId = userId;
    }
}
