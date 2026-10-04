using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using WebDav.Models;

namespace WebDav.Methods;

internal class PropFindMethod(IWebDavStorage storage) : IWebDavMethod
{
    private static readonly XNamespace Dav = "DAV:";

    public async Task HandleAsync(HttpContext context)
    {
        var segments = RequestPath.Segments(context);
        var target = storage.Get(segments);
        if (target == null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        var responses = new List<XElement> { Response(segments, target) };

        if (target.IsFolder && context.Request.Headers["Depth"] != "0")
        {
            var children = storage.List(segments);
            if (children == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            responses.AddRange(children.Select(child => Response([..segments, child.Name], child)));
        }

        var xml = new XElement(Dav + "multistatus", new XAttribute(XNamespace.Xmlns + "D", Dav), responses);

        context.Response.StatusCode = StatusCodes.Status207MultiStatus;
        context.Response.ContentType = "application/xml; charset=utf-8";
        await context.Response.WriteAsync("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + xml.ToString(SaveOptions.DisableFormatting));
    }

    private static XElement Response(string[] segments, WebDavItem item)
    {
        var props = new List<XElement>
        {
            new(Dav + "displayname", item.Name),
            new(Dav + "resourcetype", item.IsFolder ? new XElement(Dav + "collection") : null),
        };
        if (!item.IsFolder)
            props.Add(new XElement(Dav + "getcontentlength", item.Size));

        return new XElement(Dav + "response",
            new XElement(Dav + "href", Href(segments, item.IsFolder)),
            new XElement(Dav + "propstat",
                new XElement(Dav + "prop", props),
                new XElement(Dav + "status", "HTTP/1.1 200 OK")));
    }

    private static string Href(string[] segments, bool isFolder)
    {
        var href = "/" + string.Join("/", segments.Select(Uri.EscapeDataString));
        return isFolder && href != "/" ? href + "/" : href;
    }
}
