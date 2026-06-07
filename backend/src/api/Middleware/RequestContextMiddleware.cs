namespace PlatformBanking.Api.Middleware;

public sealed class RequestContextMiddleware(RequestDelegate next, ILogger<RequestContextMiddleware> logger)
{
    private const string CorrelationHeaderName = "x-correlation-id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationHeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
            context.Request.Headers[CorrelationHeaderName] = correlationId;
        }

        context.Response.Headers[CorrelationHeaderName] = correlationId;
        context.Items[CorrelationHeaderName] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["CorrelationId"] = correlationId,
                   ["Path"] = context.Request.Path
               }))
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception while processing request.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred.",
                    correlationId
                });
            }
        }
    }
}

public static class RequestContextMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestContext(this IApplicationBuilder app)
        => app.UseMiddleware<RequestContextMiddleware>();
}
