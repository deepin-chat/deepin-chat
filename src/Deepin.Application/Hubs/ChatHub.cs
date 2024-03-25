using Deepin.Application.Commands.Chats;
using Deepin.Application.Commands.Messages;
using Deepin.Application.Commands.Users;
using Deepin.Application.Models.Chats;
using Deepin.Application.Models.Messages;
using Deepin.Application.Queries;
using Deepin.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Deepin.Application.Hubs;


[Authorize]
public class ChatHub(
    IMediator mediator,
    IUserContext userContext,
    IChatQueries chatQueries,
    IChatService chatService,
    ILogger<ChatHub> logger) : Hub
{
    private readonly IMediator _mediator = mediator;
    private readonly IUserContext _userContext = userContext;
    private readonly IChatQueries _chatQueries = chatQueries;
    private readonly IChatService _chatService = chatService;
    private readonly ILogger<ChatHub> _logger = logger;
    public async override Task OnConnectedAsync()
    {
        try
        {
            await _mediator.Send(new UserOnlineCommand());
            var chats = await _chatQueries.GetChats(_userContext.UserId);
            foreach (var chatId in chats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
            }
            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connected.");
        }
    }
    public async override Task OnDisconnectedAsync(Exception exception)
    {
        try
        {
            await _mediator.Send(new UserOfflineCommand());
            var chats = await _chatQueries.GetChats(_userContext.UserId);
            foreach (var chatId in chats)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
            }
            await base.OnDisconnectedAsync(exception);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to disconnected.");
        }
    }
    public async Task SendMessage(MessageRequest request)
    {
        try
        {
            var hasPermission = await _chatService.HasPermission(request.ChatId, _userContext.UserId);
            if (!hasPermission) return;
            await _mediator.Send(new SendMessageCommand(request));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message.");
        }
    }
    public async Task ReadMessage(long chatId, ReadMessageRequest request)
    {
        try
        {
            var hasPermission = await _chatService.HasPermission(chatId, _userContext.UserId);
            if (!hasPermission) return;
            await _mediator.Send(new ReadMessageCommand(chatId, _userContext.UserId, request.MessageId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read message.");
        }
    }
}