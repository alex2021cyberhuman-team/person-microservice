using Conduit.Person.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Person.WebApi.Controllers
{
    [ApiController]
    [Route("profiles")]
    public class ProfilesController : ControllerBase
    {
        [HttpGet("test")]
        [Authorize]
        public IActionResult GetCurrentUserId()
        {
            return Ok(HttpContext.GetCurrentUserId());
        }
    }
}
