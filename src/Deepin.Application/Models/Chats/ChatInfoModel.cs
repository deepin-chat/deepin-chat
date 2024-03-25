namespace Deepin.Application.Models.Chats;
public class ChatInfoModel
{
    public string Name { get; set; }
    public string PictureId { get; set; }
    public string Picture { get; set; }
    public string Link { get; set; }
    public string Description { get; set; }
    public bool IsPrivate { get; set; }
    public string OwnerId { get; set; }
    public string[] AdminIds { get; set; }
}
