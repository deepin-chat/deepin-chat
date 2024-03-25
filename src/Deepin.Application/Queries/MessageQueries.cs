using AutoMapper;
using Deepin.Application.Constants;
using Deepin.Application.Models.Messages;
using Deepin.Application.Pagination;
using Deepin.Domain.Entities;
using Deepin.Infrastructure.Caching;
using Deepin.Infrastructure.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Deepin.Application.Queries;
public class MessageQueries(
    IMapper mapper,
    IDocumentRepository<ChatMessage> messageRepository,
    IUserQueries userQueries,
    ICacheManager cacheManager) : IMessageQueries
{
    private readonly IMapper _mapper = mapper;
    private readonly IDocumentRepository<ChatMessage> _messageRepository = messageRepository;
    private readonly IUserQueries _userQueries = userQueries;
    private readonly ICacheManager _cacheManager = cacheManager;
    public async Task<MessageModel> GetMessage(string id)
    {
        var message = await _messageRepository.FindByIdAsync(id);
        return await GetMessage(message);
    }
    public async Task<MessageModel> GetMessage(ChatMessage message)
    {
        if (message == null) return null;
        var model = _mapper.Map<MessageModel>(message);
        var user = await _userQueries.GetUser(message.From);
        model.FromUser = user?.Profile;
        return model;
    }
    public IFindFluent<ChatMessage, ChatMessage> QueryMessages(long chatId, string from = null, string keywords = null)
    {
        var filterBuilder = Builders<ChatMessage>.Filter;
        var filters = new List<FilterDefinition<ChatMessage>>();
        if (chatId > 0)
        {
            filters.Add(filterBuilder.Eq(d => d.ChatId, chatId));
        }
        if (!string.IsNullOrEmpty(from))
        {
            filters.Add(filterBuilder.Eq(d => d.From, from));
        }
        if (!string.IsNullOrEmpty(keywords))
        {
            filters.Add(filterBuilder.Regex(d => d.Text, new BsonRegularExpression(keywords, "i")));
        }
        var filter = filterBuilder.And(filters);

        var query = _messageRepository.Collection.Find(filter);
        return query;
    }
    public async Task<IPagedResult<MessageModel>> GetPagedMessages(long chatId, string from = null, string keywords = null, int pageIndex = 0, int pageSize = 10)
    {
        var query = QueryMessages(chatId, from, keywords);
        var totalCount = await query.CountDocumentsAsync();
        var documents = await query.SortByDescending(s => s.CreatedAt).Skip((pageIndex) * pageSize).Limit(pageSize).ToListAsync();
        var users = await _userQueries.GetUsers(documents.Select(d => d.From).Distinct().ToArray());
        var list = documents.Select(d =>
        {
            var user = users.FirstOrDefault(s => s.Id == d.From);
            var model = _mapper.Map<MessageModel>(d);
            model.FromUser = user?.Profile;
            return model;
        });
        return new PagedResult<MessageModel>(list.OrderBy(s => s.CreatedAt), pageIndex, pageSize, (int)totalCount);
    }

    public async Task<MessageModel> GetLastMessage(long chatId)
    {
        return await _cacheManager.GetOrCreateAsync(CacheKeys.GetLatestMessageCacheKey(chatId), async () =>
        {
            var mesage = await this.QueryMessages(chatId).SortByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
            return await GetMessage(mesage);
        });
    }
    public async Task<long> GetUnreadMessageCount(long chatId, string messageId)
    {
        var filterBuilder = Builders<ChatMessage>.Filter;
        var filters = new List<FilterDefinition<ChatMessage>>
        {
            filterBuilder.Eq(d => d.ChatId, chatId)
        };
        if (!string.IsNullOrEmpty(messageId))
        {
            filterBuilder.Gt(d => d.Id, messageId);
        }
        var filter = filterBuilder.And(filters);

        var count = await _messageRepository.Collection.CountDocumentsAsync(filter);
        return count;
    }
}