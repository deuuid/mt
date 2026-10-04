using Microsoft.AspNetCore.Http;
using WebDav.Methods;

namespace WebDav;

public class WebDavHandler
{
    private readonly Dictionary<string, IWebDavMethod> _methods;

    public WebDavHandler(IWebDavStorage storage)
    {
        _methods = new Dictionary<string, IWebDavMethod>(StringComparer.OrdinalIgnoreCase)
        {
            { WebDavMethods.PropFind, new PropFindMethod(storage) },
            { WebDavMethods.Get, new GetMethod(storage) },
            { WebDavMethods.Put, new PutMethod(storage) },
            { WebDavMethods.Head, new HeadMethod(storage) },
            { WebDavMethods.Delete, new DeleteMethod(storage) },
            { WebDavMethods.MkCol, new MkColMethod(storage) },
            { WebDavMethods.Copy, new CopyMethod(storage) },
            { WebDavMethods.Move, new MoveMethod(storage) },
        };
        _methods.Add(WebDavMethods.Options, new OptionsMethod(_methods.Keys));
    }

    public async Task HandleAsync(HttpContext context)
    {
        if (_methods.TryGetValue(context.Request.Method, out var method))
        {
            await method.HandleAsync(context);
            return;
        }

        context.Response.Headers.Allow = string.Join(", ", _methods.Keys);
        context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
    }
}
