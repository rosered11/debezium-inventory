using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace InventoryManagement.Domain.Utilities
{
    public static class ClaimConstant
    {
        public const string UserId = "uid";
    }
    public static class TokenExtension
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimConstant.UserId)?.Value;
        }

        public static string GetUserName(this HttpContext httpContext)
        {
            return httpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
