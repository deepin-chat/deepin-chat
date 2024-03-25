namespace Deepin.Domain.Entities;
public class ChatMessage : IDocument
{
    public string Id { get; set; }
    public long ChatId { get; set; }
    public string From { get; set; }
    public string Text { get; set; }
    public string ReplyTo { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}