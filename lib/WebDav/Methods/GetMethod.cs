using Microsoft.AspNetCore.Http;

namespace WebDav.Methods;

internal class GetMethod(IWebDavStorage storage) : IWebDavMethod
{
    public async Task HandleAsync(HttpContext context)
    {
        var segments = RequestPath.Segments(context);
        var target = storage.Get(segments);
        if (target == null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        if (target.IsFolder)
        {
            context.Response.Headers.Allow = $"{WebDavMethods.Options}, {WebDavMethods.PropFind}";
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            return;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/octet-stream";
        context.Response.ContentLength = (long)target.Size;

        await using var content = storage.Read(segments);
        await content.CopyToAsync(context.Response.Body);
    }
}
