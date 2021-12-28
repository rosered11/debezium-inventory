using Microsoft.AspNetCore.Http;

namespace InventoryManagement.Domain.Utilities
{
    public static class HttpContextExtension
    {
        public static string GetValueHeader(this IHeaderDictionary headerDictionary, string key)
        {
            if(headerDictionary.TryGetValue(key, out var headerValue))
            {
                return headerValue;
            }
            return null;
        }
    }
}
