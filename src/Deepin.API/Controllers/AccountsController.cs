using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deepin.API.Controllers;
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class AccountsController : ControllerBase
{
    [HttpGet("checkSession")]
    public IActionResult CheckSession()
    {
        return Ok();
    }
}
