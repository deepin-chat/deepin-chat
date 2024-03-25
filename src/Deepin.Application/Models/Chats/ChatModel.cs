using Deepin.Domain.Entities;

namespace Deepin.Application.Models.Chats;
public class ChatModel
{
    public long Id { get; set; }
    public string CreatedBy { get; set; }
    public ChatType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ChatInfoModel ChatInfo { get; set; }
    public List<ChatMemberModel> Members { get; set; }
    public long UnreadCount { get; set; }
    public ChatListItem ToListItem(string userId)
    {
        var item = new ChatListItem
        {
            Id = this.Id,
            UpdatedAt = this.UpdatedAt,
            Type = this.Type,
            UnreadCount = this.UnreadCount
        };
        if (this.Type == ChatType.DirectChat)
        {
            var member = this.Members.FirstOrDefault(s => s.UserId != userId);
            item.Picture = member.User.Profile.Picture;
            item.Name = member.User.Profile.Name;
        }
        else
        {
            item.Picture = ChatInfo.Picture;
            item.Name = ChatInfo.Name;
        }
        return item;
    }
}
