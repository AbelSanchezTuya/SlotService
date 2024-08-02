using System.Diagnostics;


namespace SlotService.API.REST.Common;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Log the request details
        Console.WriteLine($"Request {context.Request.Method} -> {context.Request.Path}");

        // Call the next middleware in the pipeline
        await next(context);

        // Log the response details and processing time
        stopwatch.Stop();
        Console.WriteLine(
            $"Response with Code {context.Response.StatusCode}, in {stopwatch.ElapsedMilliseconds} ms, for request {context.Request.Method} -> {context.Request.Path}");
    }
}
