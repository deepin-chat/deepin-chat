using Deepin.Application.Models.Messages;
using Deepin.Domain.Entities;

namespace Deepin.Application.Models.Chats;
public class ChatListItem
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Picture { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ChatType Type { get; set; }
    public long UnreadCount { get; set; }
    public MessageModel LastMessage { get; set; }
}
