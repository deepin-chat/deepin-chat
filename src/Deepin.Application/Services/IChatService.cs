using Deepin.Application.Models.Chats;

namespace Deepin.Application.Services;
public interface IChatService
{
    Task<ChatModel> GetChatById(long id);
    Task<IEnumerable<ChatListItem>> GetChats(string userId);
    Task<bool> HasPermission(long id, string userId);
}