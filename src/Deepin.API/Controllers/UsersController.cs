using Deepin.Application.Commands.Users;
using Deepin.Application.Models.Users;
using Deepin.Application.Queries;
using Deepin.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deepin.API.Controllers;
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class UsersController(
    IMediator mediator,
    IUserContext userContext,
    IUserQueries userQueries) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IUserContext _userContext = userContext;
    private readonly IUserQueries _userQueries = userQueries;
    [HttpGet]
    public async Task<ActionResult<UserModel>> Get()
    {
        var user = await _userQueries.GetUser(_userContext.UserId);
        return Ok(user);
    }
    [HttpPut("profile")]
    public async Task<ActionResult<UserProfile>> Update([FromBody] UserProfileRequest request)
    {
        var profile = await _mediator.Send(new UpdateUserProfileCommand(_userContext.UserId, request));
        return Ok(profile);
    }
}
