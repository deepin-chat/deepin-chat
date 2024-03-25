namespace Deepin.Application.Models.Messages;
public class MessageRequest
{
    public long ChatId { get; set; }
    public string Text { get; set; }
    public string ReplyTo { get; set; }
}
