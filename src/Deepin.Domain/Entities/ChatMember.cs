namespace Deepin.Domain.Entities;
public class ChatMember : Entity
{
    public static string TableName => "chat_member";
    public long ChatId { get; set; }
    public string UserId { get; set; }
    public string LastReadMessageId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
