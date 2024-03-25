using Deepin.Application.Commands.Messages;
using Deepin.Application.Models.Messages;
using Deepin.Application.Pagination;
using Deepin.Application.Queries;
using Deepin.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Deepin.API.Controllers;
[Route("api/v1/[controller]")]
[ApiController]
public class MessagesController(
    IMediator mediator, 
    IUserContext userContext, 
    IChatService chatService,
    IMessageQueries messageQueries
    ) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IUserContext _userContext = userContext;
    private readonly IChatService _chatService = chatService;
    private readonly IMessageQueries _messageQueries = messageQueries;

    [HttpGet("{id}")]
    public async Task<ActionResult<MessageModel>> Get(string id)
    {
        var model = await _messageQueries.GetMessage(id);
        var hasPermission = await _chatService.HasPermission(model.ChatId, _userContext.UserId);
        if (!hasPermission) return Forbid();
        if (model == null) return NotFound();
        return Ok(model);
    }
    [HttpGet]
    public async Task<ActionResult<IPagedResult<MessageModel>>> GetPagedMessages([FromQuery] MessageQuery query)
    {
        var hasPermission = await _chatService.HasPermission(query.ChatId, _userContext.UserId);
        if (!hasPermission) return Forbid();
        var model = await _messageQueries.GetPagedMessages(query.ChatId, query.From, query.Keywords, query.PageIndex, query.PageSize);
        return Ok(model);
    }
    [HttpPost]
    public async Task<ActionResult<MessageModel>> Post([FromBody] MessageRequest request)
    {
        var hasPermission = await _chatService.HasPermission(request.ChatId, _userContext.UserId);
        if (!hasPermission) return Forbid();
        var model = await _mediator.Send(new SendMessageCommand(request));
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<MessageModel>> Delete(string id)
    {
        var message = await _messageQueries.GetMessage(id);
        if (message.From != _userContext.UserId)
            return Forbid();
        await _mediator.Send(new DeleteMessageCommand(id));
        return NoContent();
    }
}
