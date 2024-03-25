using AutoMapper;
using Deepin.Application.Models.Chats;
using Deepin.Domain.Entities;

namespace Deepin.Application.MappingProfiles;
public class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        CreateMap<ChatInfoRequest, ChatInfo>(MemberList.Source);

        CreateMap<Chat, ChatModel>(MemberList.Destination)
            .ForMember(d => d.UnreadCount, o => o.Ignore());
        CreateMap<ChatInfo, ChatInfoModel>(MemberList.Destination)
            .ForMember(d => d.AdminIds, o => o.MapFrom((s, d) => s.AdminIds?.Split(",")));
        CreateMap<ChatMember, ChatMemberModel>(MemberList.Destination)
            .ForMember(d => d.User, o => o.Ignore());
        //CreateMap<GroupChatModel, ChatListItem>(MemberList.Destination)
        //    .ForMember(d => d.Type, o => o.MapFrom(s => ChatType.GroupChat))
        //    .ForMember(d => d.LastMessage, o => o.Ignore());

        //CreateMap<DirectChat, DirectChatModel>(MemberList.Destination);
        //CreateMap<DirectChatMember, DirectChatMemberModel>(MemberList.Destination)
        //    .ForMember(d => d.User, o => o.Ignore());
    }
}
