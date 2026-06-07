using PlatformBanking.Models;

namespace PlatformBanking.Api.Middleware;

public sealed class AuthSessionMiddleware(RequestDelegate next)
{
    private const string CorrelationHeaderName = "x-correlation-id";
    private const string UserHeaderName = "x-user-id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationHeaderName].FirstOrDefault();
        var userId = context.Request.Headers[UserHeaderName].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(correlationId))
        {
            var session = new CustomerIdentitySession(
                SessionId: Guid.NewGuid(),
                UserId: userId,
                AuthMethod: AuthMethod.EntraOidc,
                AssuranceLevel: AssuranceLevel.Medium,
                IssuedAt: DateTimeOffset.UtcNow,
                ExpiresAt: DateTimeOffset.UtcNow.AddHours(1),
                Status: SessionStatus.Active,
                CorrelationId: correlationId);

            if (!session.IsActive(DateTimeOffset.UtcNow))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Session is not active." });
                return;
            }

            context.Items[nameof(CustomerIdentitySession)] = session;
        }

        await next(context);
    }
}

public static class AuthSessionMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthSession(this IApplicationBuilder app)
        => app.UseMiddleware<AuthSessionMiddleware>();
}
