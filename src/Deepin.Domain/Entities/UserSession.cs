namespace Deepin.Domain.Entities;
public class UserSession : IDocument
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActiveAt { get; set; }
    public bool IsActive { get; set; } //=> (DateTime.UtcNow - LastActiveAt).TotalMinutes < 2;
}
