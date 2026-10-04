using Microsoft.AspNetCore.Http;

namespace WebDav.Methods;

internal interface IWebDavMethod
{
    Task HandleAsync(HttpContext context);
}
