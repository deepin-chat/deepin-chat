using AutoMapper;
using Deepin.Application.Models.Messages;
using Deepin.Domain.Entities;

namespace Deepin.Application.MappingProfiles;
public class MessageMappingProfile : Profile
{
    public MessageMappingProfile()
    {
        CreateMap<MessageRequest, ChatMessage>(MemberList.Source);

        CreateMap<ChatMessage, MessageModel>(MemberList.Destination)
            .ForMember(d => d.FromUser, o => o.Ignore());
    }
}
