using Microsoft.AspNetCore.Http;
namespace Warehouse.Infrastructure.Extensions;

public static class HttpContextExtensions
{
    public static int GetClientId(this HttpContext httpContext)
    {
        return int.Parse(httpContext.User.Claims.First(x => x.Type == "ClientId").Value);
    }
}