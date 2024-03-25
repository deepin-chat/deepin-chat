using Deepin.Application.Models.Chats;
using MediatR;

namespace Deepin.Application.Commands.Chats;
public class AddGroupChatCommand : IRequest<ChatModel>
{
    public ChatInfoRequest ChatInfo { get; set; }
    public AddGroupChatCommand(ChatInfoRequest chatInfo)
    {
        ChatInfo = chatInfo;
    }
}
public class UpdateGroupChatCommand : IRequest<ChatModel>
{
    public long Id { get; set; }
    public ChatInfoRequest ChatInfo { get; set; }
    public UpdateGroupChatCommand(long id, ChatInfoRequest chatInfo)
    {
        Id = id;
        ChatInfo = chatInfo;
    }
}
public class AddDirectChatCommand : IRequest<ChatModel>
{
    public string UserId { get; set; }
    public AddDirectChatCommand(string userId)
    {
        UserId = userId;
    }
}
public class DeleteChatCommand : IRequest<Unit>
{
    public long Id { get; set; }
    public DeleteChatCommand(long id)
    {
        Id = id;
    }
}
public class LeaveChatCommand : IRequest<Unit>
{
    public long Id { get; set; }
    public LeaveChatCommand(long id)
    {
        Id = id;
    }
}
public class ReadMessageCommand : IRequest<Unit>
{
    public long Id { get; set; }
    public string UserId { get; set; }
    public string MessageId { get; set; }
    public ReadMessageCommand(long id, string userId, string messageId)
    {
        Id = id;
        MessageId = messageId;
        UserId = userId;

    }
}