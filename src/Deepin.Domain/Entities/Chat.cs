namespace Deepin.Domain.Entities;
public class Chat : Entity
{
    public static string TableName => "chat";
    private ICollection<ChatMember> _members;
    public string CreatedBy { get; set; }
    public ChatType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public ChatInfo ChatInfo { get; private set; }
    public ICollection<ChatMember> Members
    {
        get => _members ??= [];
        private set => _members = value;
    }
    public void AddMember(string userId)
    {
        this.Members.Add(new ChatMember
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
        });
    }
    public void RemoveMember(string userId)
    {
        var member = this.Members.FirstOrDefault(x => x.UserId == userId);
        if (member != null)
        {
            this.Members.Remove(member);
        }
    }
    public void SetChatInfo(ChatInfo chatInfo)
    {
        this.ChatInfo = chatInfo;
    }
}
