using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace WebDav.Methods;

internal static class RequestPath
{
    public static string[] Segments(HttpContext context)
    {
        return Uri.UnescapeDataString(RawPath(context)).Split('/', StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[]? DestinationSegments(HttpContext context)
    {
        var destination = context.Request.Headers["Destination"].ToString();
        if (string.IsNullOrEmpty(destination))
            return null;

        var path = Uri.TryCreate(destination, UriKind.Absolute, out var uri) ? uri.AbsolutePath : destination;
        var query = path.IndexOf('?');
        if (query >= 0)
            path = path[..query];

        return Uri.UnescapeDataString(path).Split('/', StringSplitOptions.RemoveEmptyEntries);
    }

    private static string RawPath(HttpContext context)
    {
        var target = context.Features.Get<IHttpRequestFeature>()?.RawTarget ?? "/";
        var query = target.IndexOf('?');
        return query < 0 ? target : target[..query];
    }
}
