using Microsoft.AspNetCore.Http;

namespace WebDav.Methods;

internal class HeadMethod(IWebDavStorage storage) : IWebDavMethod
{
    public Task HandleAsync(HttpContext context)
    {
        var target = storage.Get(RequestPath.Segments(context));
        if (target == null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }

        if (target.IsFolder)
        {
            context.Response.Headers.Allow = $"{WebDavMethods.Options}, {WebDavMethods.PropFind}";
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            return Task.CompletedTask;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/octet-stream";
        context.Response.ContentLength = (long)target.Size;
        return Task.CompletedTask;
    }
}
