using Deepin.Domain.Entities;

namespace Deepin.Application.Queries;
public interface IChatQueries
{
    Task<Chat> GetChatById(long id);
    Task<IEnumerable<long>> GetChats(string userId);
    Task<string> GetLastReadMessageId(long chatId, string userId);
}