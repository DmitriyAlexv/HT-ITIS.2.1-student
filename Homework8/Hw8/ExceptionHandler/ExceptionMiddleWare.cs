namespace Hw8.ExceptionHandler;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;    
    
    public ExceptionMiddleware(RequestDelegate next)    
    {    
        _next = next;    
    }    
    
    public async Task Invoke(HttpContext context)    
    {    
        try    
        {    
            await _next.Invoke(context);    
        }    
        catch (Exception ex)
        {
            context.Response.Headers["Content-Type"] = "text/plain; charset=utf-8";
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(ex.Message);
        }    
    }    
}