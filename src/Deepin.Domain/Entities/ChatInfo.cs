namespace Deepin.Domain.Entities;
public class ChatInfo : ValueObject
{
    public string Name { get; set; }
    public string PictureId { get; set; }
    public string Link { get; set; }
    public string Description { get; set; }
    public bool IsPrivate { get; set; }
    public string OwnerId { get; set; }
    public string AdminIds { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return new object[] { Name, PictureId, Link, Description,  OwnerId, AdminIds, IsPrivate };
    }
}
