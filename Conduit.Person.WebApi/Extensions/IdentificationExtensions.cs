using System.Security.Claims;

namespace Conduit.Person.WebApi.Extensions;

public static class IdentificationExtensions
{
    public static string GetCurrentUserId(
        this HttpContext httpContext)
    {
        return GetCurrentUserIdOptional(httpContext) ??
               throw new ApplicationException("Empty identification claim");
    }

    public static string? GetCurrentUserIdOptional(
        this HttpContext httpContext)
    {
        var claim = GetClaim(httpContext);

        var currentUserId = claim?.Value;

        return currentUserId;
    }

    private static Claim? GetClaim(
        HttpContext httpContext)
    {
        return FindFirst(httpContext);
    }

    private static Claim? FindFirst(
        HttpContext httpContext)
    {
        return httpContext.User.Identity?.IsAuthenticated == true
            ? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)
            : null;
    }
}
