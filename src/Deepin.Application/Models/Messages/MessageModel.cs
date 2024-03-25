using Deepin.Application.Models.Users;

namespace Deepin.Application.Models.Messages;
public class MessageModel
{
    public string Id { get; set; }
    public long ChatId { get; set; }
    public string From { get; set; }
    public string Text { get; set; }
    public string ReplyTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public UserProfile FromUser { get; set; }
}
