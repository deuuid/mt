using Microsoft.AspNetCore.Http;

namespace WebDav.Methods;

internal class DeleteMethod(IWebDavStorage storage) : IWebDavMethod
{
    public Task HandleAsync(HttpContext context)
    {
        var segments = RequestPath.Segments(context);
        if (segments.Length == 0)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        if (storage.Get(segments) == null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }

        storage.Delete(segments);
        context.Response.StatusCode = StatusCodes.Status204NoContent;
        return Task.CompletedTask;
    }
}
