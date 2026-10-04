using Microsoft.AspNetCore.Http;

namespace WebDav.Methods;

internal class PutMethod(IWebDavStorage storage) : IWebDavMethod
{
    public Task HandleAsync(HttpContext context)
    {
        var segments = RequestPath.Segments(context);
        if (segments.Length == 0)
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            return Task.CompletedTask;
        }

        var parent = storage.Get(segments[..^1]);
        if (parent is not { IsFolder: true })
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            return Task.CompletedTask;
        }

        var existing = storage.Get(segments);
        if (existing is { IsFolder: true })
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            return Task.CompletedTask;
        }

        if (context.Request.ContentLength is not { } length)
        {
            context.Response.StatusCode = StatusCodes.Status411LengthRequired;
            return Task.CompletedTask;
        }

        storage.Write(segments, context.Request.Body, length);
        context.Response.StatusCode = existing == null ? StatusCodes.Status201Created : StatusCodes.Status204NoContent;
        return Task.CompletedTask;
    }
}
