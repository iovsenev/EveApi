namespace Eve.Api.Minddlewares;

public class ClientIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _allowedClient = "frontend";

    public ClientIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("X-Client-Id"))
        {
            var clientId = context.Request.Headers["X-Client-Id"].ToString();
            if (clientId != _allowedClient)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden: Invalid client id");
                return;
            }
        }
        else
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Bad Request: Missing X-Client-Id header");
            return;
        }

        await _next(context);
    }

}
