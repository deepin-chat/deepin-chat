using Deepin.Application.Commands.Chats;
using Deepin.Application.Models.Chats;
using Deepin.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deepin.API.Controllers;
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class ChatsController(IMediator mediator, IUserContext userContext, IChatService chatService) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IUserContext _userContext = userContext;
    private readonly IChatService _chatService = chatService;
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatListItem>>> GetChats()
    {
        var chats = await _chatService.GetChats(_userContext.UserId);
        return Ok(chats);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ChatModel>> Get(long id)
    {
        var hasPermission = await _chatService.HasPermission(id, _userContext.UserId);
        if (!hasPermission) return NotFound();
        var chat = await _chatService.GetChatById(id);
        if (chat == null)
            return NotFound();
        return Ok(chat);
    }

    [HttpPost("group")]
    public async Task<ActionResult<ChatModel>> CreateGroupChat([FromBody] ChatInfoRequest request)
    {
        var chat = await _mediator.Send(new AddGroupChatCommand(request));
        return CreatedAtAction(nameof(Get), new { id = chat.Id }, chat);
    }

    [HttpPut("{id}/group")]
    public async Task<IActionResult> UpdateGroupChat(long id, [FromBody] ChatInfoRequest request)
    {
        var hasPermission = await _chatService.HasPermission(id, _userContext.UserId);
        if (!hasPermission) return NotFound();
        var model = await _mediator.Send(new UpdateGroupChatCommand(id, request));
        return Ok(model);
    }

    [HttpPost("direct")]
    public async Task<ActionResult<ChatModel>> CreatePrivateChat([FromBody] AddDirectChatCommand command)
    {
        var chat = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = chat.Id }, chat);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var hasPermission = await _chatService.HasPermission(id, _userContext.UserId);
        if (!hasPermission) return NotFound();
        await _mediator.Send(new DeleteChatCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/leave")]
    public async Task<IActionResult> LeaveChat(long id)
    {
        var hasPermission = await _chatService.HasPermission(id, _userContext.UserId);
        if (!hasPermission) return NotFound();
        await _mediator.Send(new LeaveChatCommand(id));
        return Ok();
    }
    [HttpPost("{id}/read")]
    public async Task<IActionResult> ReadMessage(long id, [FromBody] ReadMessageRequest request)
    {
        var hasPermission = await _chatService.HasPermission(id, _userContext.UserId);
        if (!hasPermission) return NotFound();
        await _mediator.Send(new ReadMessageCommand(id, _userContext.UserId, request.MessageId));
        return Ok();
    }
}
