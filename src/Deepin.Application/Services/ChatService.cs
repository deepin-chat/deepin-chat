using AutoMapper;
using Deepin.Application.Constants;
using Deepin.Application.Models.Chats;
using Deepin.Application.Queries;
using Deepin.Domain.Entities;
using Deepin.Infrastructure.Caching;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;

namespace Deepin.Application.Services;
public class ChatToken
{
    public string UserId { get; set; }
    public long Id { get; set; }
    public string[] AdminIds { get; set; }
}
public class ChatService(
    IMapper mapper,
    IUserContext userContext,
    IUserQueries userQueries,
    IChatQueries chatQueries,
    IMessageQueries messageQueries,
    ICacheManager cacheManager,
    IFileService fileService,
    IDataProtectionProvider dataProtectionProvider) : IChatService
{
    private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector(nameof(ChatService));
    private readonly IMapper _mapper = mapper;
    private readonly IUserContext _userContext = userContext;
    private readonly IUserQueries _userQueries = userQueries;
    private readonly IChatQueries _chatQueries = chatQueries;
    private readonly IMessageQueries _messageQueries = messageQueries;
    private readonly ICacheManager _cacheManager = cacheManager;
    private readonly IFileService _fileService = fileService;
    public async Task<IEnumerable<ChatListItem>> GetChats(string userId)
    {
        var chats = new List<ChatListItem>();
        var chatIds = await _chatQueries.GetChats(userId);
        foreach (var chatId in chatIds)
        {
            var chat = await GetChatById(chatId);
            chats.Add(chat.ToListItem(_userContext.UserId));
        }
        foreach (var chat in chats)
        {
            chat.LastMessage = await _messageQueries.GetLastMessage(chat.Id);
            if (chat.LastMessage != null && chat.LastMessage.CreatedAt > chat.UpdatedAt)
            {
                chat.UpdatedAt = chat.LastMessage.CreatedAt;
            }
        }
        return chats.OrderByDescending(s => s.UpdatedAt);
    }
    public async Task<ChatModel> GetChatById(long id)
    {
        var chat = await _cacheManager.GetOrCreateAsync(CacheKeys.GetChatCacheKey(id), async () =>
        {
            var entity = await _chatQueries.GetChatById(id);
            return _mapper.Map<ChatModel>(entity);
        });
        var lastReadMessageId = await _chatQueries.GetLastReadMessageId(id, _userContext.UserId);
        chat.UnreadCount = await _messageQueries.GetUnreadMessageCount(id, lastReadMessageId);
        if (chat.Type == ChatType.GroupChat)
        {
            chat.ChatInfo.Picture = _fileService.GenerateTemporaryUrl(chat.ChatInfo.PictureId);
        }
        if (chat.Type == ChatType.DirectChat)
        {
            foreach (var m in chat.Members)
            {
                m.User = await _userQueries.GetUser(m.UserId);
            }
        }
        return chat;
    }
    //private string ProtectChatToken(Chat chat)
    //{
    //    var token = new ChatToken
    //    {
    //        Id = chat.Id,
    //        UserId = _userContext.UserId
    //    };
    //    if (chat.Type == ChatType.GroupChat)
    //    {
    //        token.AdminIds = chat.ChatInfo.AdminIds.Split(",");
    //    }
    //    return _protector.Protect(JsonConvert.SerializeObject(token));
    //}
    //private ChatToken UnprotectChatToken(string token)
    //{
    //    if (string.IsNullOrEmpty(token))
    //        return null;
    //    var text = _protector.Unprotect(token);
    //    return JsonConvert.DeserializeObject<ChatToken>(text);
    //}
    //public bool ValidateChatToken(string tokenText, long id, string userId)
    //{
    //    var token = UnprotectChatToken(tokenText);
    //    if (token == null)
    //        return false;
    //    if (token.Id != id)
    //        return false;
    //    if (userId != token.UserId)
    //        return false;
    //    return true;
    //}
    //public bool HasAdminPermission(long id, string token, string userId)
    //{
    //    var text = _protector.Unprotect(token);
    //    if (string.IsNullOrEmpty(text) || !text.Contains(":"))
    //        return false;
    //    var list = text.Split(':');
    //    if (list[0] != id.ToString())
    //        return false;
    //    var admins = list[1].Split(",");
    //    return admins.Contains(userId);
    //}

    public async Task<bool> HasPermission(long id, string userId)
    {
        var chats = await _chatQueries.GetChats(userId);
        return chats.Any(i => i == id);
    }
}
