using System.Security.Claims;

namespace Conduit.Person.WebApi.Extensions;

public static class IdentificationExtensions
{
    public static Guid GetCurrentUserId(
        this HttpContext httpContext)
    {
        var stringValue = GetCurrentUserIdOptional(httpContext) ??
                          throw new ApplicationException(
                              "Empty identification claim");
        var typedValue = Guid.Parse(stringValue);
        return typedValue;
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
