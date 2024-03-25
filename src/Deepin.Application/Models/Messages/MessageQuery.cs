using Deepin.Application.Pagination;

namespace Deepin.Application.Models.Messages;
public class MessageQuery : PagedQuery
{
    public long ChatId { get; set; }
    public string From { get; set; }
    public string Keywords { get; set; }
}
