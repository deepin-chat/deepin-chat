using Deepin.Application.Models.Messages;
using Deepin.Application.Pagination;
using Deepin.Domain.Entities;
using MongoDB.Driver;

namespace Deepin.Application.Queries;
public interface IMessageQueries
{
    Task<MessageModel> GetLastMessage(long chatId);
    Task<MessageModel> GetMessage(ChatMessage message);
    Task<MessageModel> GetMessage(string id);
    Task<IPagedResult<MessageModel>> GetPagedMessages(long chatId, string from = null, string keywords = null, int pageIndex = 0, int pageSize = 10);
    IFindFluent<ChatMessage, ChatMessage> QueryMessages(long chatId, string from = null, string keywords = null);
    Task<long> GetUnreadMessageCount(long chatId, string messageId);
}