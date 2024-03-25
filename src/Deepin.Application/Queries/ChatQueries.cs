using Deepin.Domain.Entities;
using Deepin.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Deepin.Application.Queries;
public class ChatQueries(DeepinDbContext db) : IChatQueries
{
    private readonly DeepinDbContext _db = db;
    public async Task<IEnumerable<long>> GetChats(string userId)
    {
        return await _db.Chats.Where(x => x.Members.Any(m => m.UserId == userId)).Select(s => s.Id).ToListAsync();
    }
    public async Task<Chat> GetChatById(long id)
    {
        var chat = await _db.Chats.FindAsync(id);
        if (chat == null)
            return null;
        var entry = _db.Entry(chat);
        if (chat.Type == ChatType.DirectChat)
        {
            await entry.Collection(c => c.Members).LoadAsync();
        }
        else
        {
            await entry.Reference(c => c.ChatInfo).LoadAsync();
        }
        return chat;
    }
    public async Task<string> GetLastReadMessageId(long chatId, string userId)
    {
        var member = await _db.ChatMembers.FirstOrDefaultAsync(s => s.ChatId == chatId && s.UserId == userId);
        return member == null ? null : member.LastReadMessageId;
    }
}