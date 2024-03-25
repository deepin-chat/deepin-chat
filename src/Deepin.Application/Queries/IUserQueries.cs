using Deepin.Application.Models.Users;
using Deepin.Domain.Entities;

namespace Deepin.Application.Queries;
public interface IUserQueries
{
    Task<UserModel> GetUser(string id);
    Task<IEnumerable<UserModel>> GetUsers(IEnumerable<string> ids);
    Task<IEnumerable<UserSession>> GetUserSessions(string userId);
}