using Microsoft.AspNetCore.Http;

namespace WebDav.Methods;

internal class MoveMethod(IWebDavStorage storage) : IWebDavMethod
{
    public Task HandleAsync(HttpContext context)
    {
        var source = RequestPath.Segments(context);
        var destination = RequestPath.DestinationSegments(context);
        if (destination == null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return Task.CompletedTask;
        }

        if (storage.Get(source) == null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }

        if (source.Length == 0 || destination.Length == 0 || source.SequenceEqual(destination))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        if (storage.Get(destination[..^1]) is not { IsFolder: true })
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            return Task.CompletedTask;
        }

        var existing = storage.Get(destination);
        if (existing != null)
        {
            if (context.Request.Headers["Overwrite"] == "F")
            {
                context.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
                return Task.CompletedTask;
            }

            storage.Delete(destination);
        }

        storage.Move(source, destination);
        context.Response.StatusCode = existing == null ? StatusCodes.Status201Created : StatusCodes.Status204NoContent;
        return Task.CompletedTask;
    }
}
