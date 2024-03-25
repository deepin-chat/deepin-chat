using AutoMapper;
using Deepin.Application.Constants;
using Deepin.Application.Models.Users;
using Deepin.Application.Services;
using Deepin.Domain.Entities;
using Deepin.Infrastructure;
using Deepin.Infrastructure.Caching;
using Deepin.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Deepin.Application.Queries;
public class UserQueries(IMapper mapper, ICacheManager cacheManager, UserManager<User> userManager, DeepinDbContext db, IFileService fileService,
    IDocumentRepository<UserSession> userSessionRepository) : IUserQueries
{
    private readonly IMapper _mapper = mapper;
    private readonly ICacheManager _cacheManager = cacheManager;
    private readonly UserManager<User> _userManager = userManager;
    private readonly DeepinDbContext _db = db;
    private readonly IFileService _fileService = fileService;
    private readonly IDocumentRepository<UserSession> _userSessionRepository = userSessionRepository;
    public async Task<UserModel> GetUser(string id)
    {
        return await _cacheManager.GetOrCreateAsync(CacheKeys.GetUserCacheKey(id), async () =>
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;
            var claims = await _userManager.GetClaimsAsync(user);
            var model = _mapper.Map<UserModel>(user);
            if (claims.Any())
            {
                model.Profile = _mapper.Map<UserProfile>(claims);
                if (!string.IsNullOrEmpty(model.Profile.Picture))
                {
                    model.Profile.Picture = _fileService.GenerateTemporaryUrl(model.Profile.Picture);
                }
            }
            return model;
        });
    }

    public async Task<IEnumerable<UserModel>> GetUsers(IEnumerable<string> ids)
    {
        var tasks = ids.Select(this.GetUser).ToList();
        return (await Task.WhenAll(tasks)).Where(s => s != null);
    }

    public async Task<IEnumerable<UserSession>> GetUserSessions(string userId)
    {
        return await _cacheManager.GetOrCreateAsync(CacheKeys.GetUserSessionsCacheKey(userId), async () =>
        {
            var filterBuilder = Builders<UserSession>.Filter;
            var filters = new List<FilterDefinition<UserSession>>
          {
            filterBuilder.Eq(d => d.UserId, userId)
          };
            var filter = filterBuilder.And(filters);
            var query = await _userSessionRepository.Collection.FindAsync(filter);
            return await query.ToListAsync();
        });
    }
}
