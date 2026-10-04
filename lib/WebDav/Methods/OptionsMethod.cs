using Microsoft.AspNetCore.Http;

namespace WebDav.Methods;

internal class OptionsMethod(IEnumerable<string> allowedMethods) : IWebDavMethod
{
    public Task HandleAsync(HttpContext context)
    {
        context.Response.Headers.Allow = string.Join(", ", allowedMethods);
        context.Response.Headers["DAV"] = "1";
        context.Response.StatusCode = StatusCodes.Status200OK;
        return Task.CompletedTask;
    }
}
