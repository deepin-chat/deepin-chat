using AutoMapper;
using Deepin.Application.Constants;
using Deepin.Application.IntegrationEvents.Events;
using Deepin.Application.Models.Chats;
using Deepin.Application.Models.Messages;
using Deepin.Application.Queries;
using Deepin.Application.Services;
using Deepin.Domain.Entities;
using Deepin.Infrastructure.Caching;
using Deepin.Infrastructure.Repositories;
using MassTransit;
using MediatR;

namespace Deepin.Application.Commands.Messages;
public class MessagesCommandHandler(
    IMapper mapper,
    IBus bus,
    ICacheManager cacheManager,
    IDocumentRepository<ChatMessage> messageRepository,
    IUserContext userContext,
    IMessageQueries messageQueries) : IRequestHandler<SendMessageCommand, MessageModel>, IRequestHandler<DeleteMessageCommand, Unit>
{
    private readonly IMapper _mapper = mapper;
    private readonly IBus _bus = bus;
    private readonly ICacheManager _cacheManager = cacheManager;
    private readonly IDocumentRepository<ChatMessage> _messageRepository = messageRepository;
    private readonly IUserContext _userContext = userContext;
    private readonly IMessageQueries _messageQueries = messageQueries;
    public async Task<MessageModel> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = _mapper.Map<ChatMessage>(request.Message);
        message.From = _userContext.UserId;
        await _messageRepository.InsertAsync(message);
        await _cacheManager.RemoveAsync(CacheKeys.GetLatestMessageCacheKey(request.Message.ChatId));
        var model = await _messageQueries.GetMessage(message);
        await _bus.Publish(new SendMessageIntegrationEvent(_userContext.UserId, model), cancellationToken);
        return model;
    }

    public async Task<Unit> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageRepository.DeleteByIdAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
