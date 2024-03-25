using Deepin.Application.Models.Users;
using Deepin.Domain.Entities;
using MediatR;

namespace Deepin.Application.Commands.Users;
public class UserOnlineCommand : IRequest<UserSession>
{
}
public class UserOfflineCommand : IRequest<UserSession>
{
}
public class UpdateUserProfileCommand : IRequest<UserProfile>
{
    public string UserId { get; set; }
    public UserProfileRequest Profile { get; set; }
    public UpdateUserProfileCommand(string userId, UserProfileRequest profile)
    {
        UserId = userId;
        Profile = profile;
    }
}