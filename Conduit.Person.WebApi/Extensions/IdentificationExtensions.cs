using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Conduit.Person.WebApi.Extensions
{
    public static class IdentificationExtensions
    {
        public static string GetCurrentUserId(this HttpContext httpContext)
        {
            var claim = GetClaim(httpContext);

            var currentUserId = claim.Value;

            return currentUserId;
        }

        private static Claim GetClaim(HttpContext httpContext)
        {
            var claim = FindFirst(httpContext);

            if (claim is null)
            {
                throw new InvalidOperationException(
                    "User is not authenticated");
            }

            if (string.IsNullOrWhiteSpace(claim.Value))
            {
                throw new InvalidOperationException(
                    $"Invalid userId ({claim.Value})");
            }

            return claim;
        }

        private static Claim? FindFirst(HttpContext httpContext)
        {
            return httpContext.User.Identity?.IsAuthenticated == true
                ? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)
                : null;
        }
    }
}
