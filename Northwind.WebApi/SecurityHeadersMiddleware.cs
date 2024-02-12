using Microsoft.Extensions.Primitives;
namespace Northwind.WebApi;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context){
        context.Response.Headers.Add("super-secure", new StringValues("enable"));
        return _next(context);
    }
}
