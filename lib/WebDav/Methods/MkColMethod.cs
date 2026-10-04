using Microsoft.AspNetCore.Http;

namespace WebDav.Methods;

internal class MkColMethod(IWebDavStorage storage) : IWebDavMethod
{
    public Task HandleAsync(HttpContext context)
    {
        var segments = RequestPath.Segments(context);
        if (segments.Length == 0 || storage.Get(segments) != null)
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            return Task.CompletedTask;
        }

        if (context.Request.ContentLength > 0)
        {
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            return Task.CompletedTask;
        }

        if (storage.Get(segments[..^1]) is not { IsFolder: true })
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            return Task.CompletedTask;
        }

        storage.CreateFolder(segments);
        context.Response.StatusCode = StatusCodes.Status201Created;
        return Task.CompletedTask;
    }
}
