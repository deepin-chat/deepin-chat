namespace Deepin.Application.Services;
public interface IUserContext
{
    string UserId { get; }
    string UserAgent { get; }
    string IpAddress { get; }
}
