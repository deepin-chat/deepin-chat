using Deepin.Application.Models.Users;

namespace Deepin.Application.Models.Chats;
public class ChatMemberModel
{
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string LastReadMessageId { get; set; }
    public UserModel User { get; set; }
}
