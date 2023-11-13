namespace Hw8.ExceptionHandler;

public static class AppExceptionExtension
{
    public static void UseExceptionMiddleware(this IApplicationBuilder app)  
    {  
        app.UseMiddleware<ExceptionMiddleware>();  
    }  
}