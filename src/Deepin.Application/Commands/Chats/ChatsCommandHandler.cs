using AutoMapper;
using Deepin.Application.Constants;
using Deepin.Application.IntegrationEvents.Events;
using Deepin.Application.Models.Chats;
using Deepin.Application.Services;
using Deepin.Domain.Entities;
using Deepin.Domain.Exceptions;
using Deepin.Infrastructure;
using Deepin.Infrastructure.Caching;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Deepin.Application.Commands.Chats;
public class ChatsCommandHandler(
    IMapper mapper,
    IBus bus,
    ICacheManager cacheManager,
    DeepinDbContext db,
    IUserContext userContext,
    IChatService chatService) :
    IRequestHandler<AddGroupChatCommand, ChatModel>,
    IRequestHandler<AddDirectChatCommand, ChatModel>,
    IRequestHandler<UpdateGroupChatCommand, ChatModel>,
    IRequestHandler<DeleteChatCommand, Unit>,
    IRequestHandler<LeaveChatCommand, Unit>,
    IRequestHandler<ReadMessageCommand, Unit>
{
    private readonly IBus _bus = bus;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheManager _cacheManager = cacheManager;
    private readonly DeepinDbContext _db = db;
    private readonly IUserContext _userContext = userContext;
    private readonly IChatService _chatService = chatService;

    public async Task<ChatModel> Handle(AddGroupChatCommand request, CancellationToken cancellationToken)
    {
        var chat = new Chat
        {
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _userContext.UserId,
            Type = ChatType.GroupChat,
            UpdatedAt = DateTime.UtcNow
        };
        chat.SetChatInfo(_mapper.Map<ChatInfo>(request.ChatInfo));
        chat.ChatInfo.OwnerId = _userContext.UserId;
        chat.ChatInfo.AdminIds = string.Join(",", [_userContext.UserId]);
        chat.AddMember(_userContext.UserId);
        await _db.Chats.AddAsync(chat);
        await _db.SaveEntitiesAsync();
        await _bus.Publish(new ChatCreatedIntegrationEvent(_userContext.UserId));
        return await _chatService.GetChatById(chat.Id);
    }

    public async Task<ChatModel> Handle(AddDirectChatCommand request, CancellationToken cancellationToken)
    {
        var chat = new Chat
        {
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _userContext.UserId,
            Type = ChatType.DirectChat,
            UpdatedAt = DateTime.UtcNow
        };
        chat.AddMember(_userContext.UserId);
        chat.AddMember(request.UserId);
        await _db.Chats.AddAsync(chat);
        await _db.SaveEntitiesAsync();
        await _bus.Publish(new ChatCreatedIntegrationEvent(_userContext.UserId));
        return await _chatService.GetChatById(chat.Id);
    }
    public async Task<Unit> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _db.Chats.FindAsync(request.Id);
        if (chat != null)
        {
            _db.Chats.Remove(chat);
            await _db.SaveEntitiesAsync(cancellationToken);
            await _cacheManager.RemoveAsync(CacheKeys.GetChatCacheKey(request.Id));
        }
        return Unit.Value;
    }

    public async Task<ChatModel> Handle(UpdateGroupChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _db.Chats.FindAsync(request.Id);
        if (chat == null)
        {
            throw new DomainException($"Chat not found {request.Id}");
        }
        await _db.Entry(chat).Reference(c => c.ChatInfo).LoadAsync();
        chat.SetChatInfo(_mapper.Map(request.ChatInfo, chat.ChatInfo));
        _db.Chats.Update(chat);
        await _db.SaveEntitiesAsync();
        await _cacheManager.RemoveAsync(CacheKeys.GetChatCacheKey(request.Id));
        return await _chatService.GetChatById(chat.Id);
    }

    public async Task<Unit> Handle(LeaveChatCommand request, CancellationToken cancellationToken)
    {
        var chatMember = await _db.ChatMembers.FirstOrDefaultAsync(s => s.ChatId == request.Id && s.UserId == _userContext.UserId);
        if (chatMember == null)
            return Unit.Value;
        _db.ChatMembers.Remove(chatMember);
        await _db.SaveChangesAsync();
        return Unit.Value;
    }

    public async Task<Unit> Handle(ReadMessageCommand request, CancellationToken cancellationToken)
    {
        var chatMember = await _db.ChatMembers.FirstOrDefaultAsync(s => s.ChatId == request.Id && s.UserId == _userContext.UserId);
        if (chatMember == null)
            return Unit.Value;
        chatMember.LastReadMessageId = request.MessageId;
        _db.ChatMembers.Update(chatMember);
        await _db.SaveChangesAsync();
        return Unit.Value;
    }
}
