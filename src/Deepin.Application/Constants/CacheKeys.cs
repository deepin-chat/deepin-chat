using Deepin.Application.Models.Chats;

namespace Deepin.Application.Constants;
public static class CacheKeys
{
    public static string GetUserCacheKey(string id) => $"user_{id}";
    public static string GetUserSessionsCacheKey(string id) => $"user_sessions_{id}";
    public static string GetLatestMessageCacheKey(long chatId) => $"latest_message_{chatId}";
    public static string GetFileByIdCacheKey(string id) => $"file_{id}";
    public static string GetChatCacheKey(long id) => $"chat_{id}";
}
