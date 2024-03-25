using AutoMapper;
using Deepin.Application.Constants;
using Deepin.Application.Models.Users;
using Deepin.Application.Queries;
using Deepin.Application.Services;
using Deepin.Domain.Entities;
using Deepin.Domain.Exceptions;
using Deepin.Infrastructure;
using Deepin.Infrastructure.Caching;
using Deepin.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;

namespace Deepin.Application.Commands.Users;
internal class UsersCommandHandler(
    IMapper mapper,
    ICacheManager cacheManager,
    UserManager<User> userManager,
    IUserQueries userQueries,
    IUserContext userContext,
    DeepinDbContext db,
    IDocumentRepository<UserSession> userSessionRepository) :
    IRequestHandler<UserOnlineCommand, UserSession>,
    IRequestHandler<UserOfflineCommand, UserSession>,
    IRequestHandler<UpdateUserProfileCommand, UserProfile>
{
    private readonly IMapper _mapper = mapper;
    private readonly ICacheManager _cacheManager = cacheManager;
    private readonly IUserQueries _userQueries = userQueries;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IUserContext _userContext = userContext;
    private readonly DeepinDbContext _db = db;
    private readonly IDocumentRepository<UserSession> _userSessionRepository = userSessionRepository;

    public async Task<UserSession> Handle(UserOnlineCommand request, CancellationToken cancellationToken)
    {
        return await this.AddOrUpdateSession(true);
    }
    public async Task<UserSession> Handle(UserOfflineCommand request, CancellationToken cancellationToken)
    {
        return await this.AddOrUpdateSession(false);
    }

    public async Task<UserProfile> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            throw new DomainException();
        var oldClaims = await _userManager.GetClaimsAsync(user);
        var userClaims = _mapper.Map<IEnumerable<Claim>>(request.Profile);
        await _userManager.RemoveClaimsAsync(user, oldClaims);
        await _userManager.AddClaimsAsync(user, userClaims);
        user.ModifiedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        await _cacheManager.RemoveAsync(CacheKeys.GetUserCacheKey(request.UserId));
        var updatedUser = await _userQueries.GetUser(request.UserId);
        return updatedUser?.Profile;
    }
    private async Task<UserSession> AddOrUpdateSession(bool isActive)
    {
        var sessions = await _userQueries.GetUserSessions(_userContext.UserId);
        var session = sessions.FirstOrDefault(x => x.UserAgent == _userContext.UserAgent);
        if (session == null)
        {
            session = new UserSession
            {
                CreatedAt = DateTime.UtcNow,
                IpAddress = _userContext.IpAddress,
                UserAgent = _userContext.UserAgent,
                UserId = _userContext.UserId,
                LastActiveAt = DateTime.UtcNow,
                IsActive = isActive
            };
            await _userSessionRepository.InsertAsync(session);
        }
        else
        {
            var filter = Builders<UserSession>.Filter.Eq(s => s.Id, session.Id);
            var update = Builders<UserSession>.Update
                .Set(s => s.LastActiveAt, DateTime.UtcNow)
                .Set(s => s.IpAddress, _userContext.IpAddress)
                .Set(s => s.IsActive, isActive);

            await _userSessionRepository.Collection.UpdateOneAsync(filter, update);
        }
        await _cacheManager.RemoveAsync(CacheKeys.GetUserSessionsCacheKey(_userContext.UserId));
        return session;
    }
}
