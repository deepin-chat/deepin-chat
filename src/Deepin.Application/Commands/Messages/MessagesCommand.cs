using Deepin.Application.Models.Messages;
using MediatR;

namespace Deepin.Application.Commands.Messages;
public record SendMessageCommand : IRequest<MessageModel>
{
    public MessageRequest Message { get; set; }
    public SendMessageCommand(MessageRequest message)
    {
        Message = message;
    }
}
public record DeleteMessageCommand : IRequest<Unit>
{
    public string Id { get; set; }
    public DeleteMessageCommand(string id)
    {
        Id = id;
    }
}