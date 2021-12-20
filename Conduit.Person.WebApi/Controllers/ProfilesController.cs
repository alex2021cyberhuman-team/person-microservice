using System.Net;
using Conduit.Person.BusinessLogic;
using Conduit.Person.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Person.WebApi.Controllers;

[ApiController]
[Route("profiles/{username}")]
public class ProfilesController : ControllerBase
{
    private readonly IFollowingsManager _followingsManager;
    private readonly IProfileViewer _profileViewer;

    public ProfilesController(
        IProfileViewer profileViewer,
        IFollowingsManager followingsManager)
    {
        _profileViewer = profileViewer;
        _followingsManager = followingsManager;
    }

    [HttpGet(Name = "getProfile")]
    public async Task<IActionResult> GetProfile(
        string username,
        CancellationToken cancellationToken)
    {
        var currentUserId = HttpContext.GetCurrentUserIdOptional();
        var result = await _profileViewer.ReturnProfile(
            new(username, Guid.Empty), cancellationToken);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return Ok(result.Response);
        }

        return StatusCode((int)result.StatusCode);
    }

    [HttpPost("follow", Name = "followUser")]
    [Authorize]
    public async Task<IActionResult> FollowUser(
        string username,
        CancellationToken cancellationToken)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        var result =
            await _followingsManager.AssignFollowingAsync(
                new(username, currentUserId), cancellationToken);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return Ok(result.Response);
        }

        return StatusCode((int)result.StatusCode);
    }

    [HttpDelete("follow", Name = "unfollowUser")]
    [Authorize]
    public async Task<IActionResult> UnfollowUser(
        string username,
        CancellationToken cancellationToken)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        var result =
            await _followingsManager.DeAssignFollowingAsync(
                new(username, currentUserId), cancellationToken);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return Ok(result.Response);
        }

        return StatusCode((int)result.StatusCode);
    }
}
