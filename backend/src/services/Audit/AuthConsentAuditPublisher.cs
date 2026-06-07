using PlatformBanking.Models;

namespace PlatformBanking.Services.Audit;

public interface IAuthConsentAuditPublisher
{
    Task PublishSignInAsync(string correlationId, string userId, CancellationToken cancellationToken = default);

    Task PublishConsentUpdateAsync(
        string correlationId,
        string userId,
        string scopeCategory,
        string status,
        CancellationToken cancellationToken = default);
}

public sealed class AuthConsentAuditPublisher(IAuditWriter auditWriter) : IAuthConsentAuditPublisher
{
    public Task PublishSignInAsync(string correlationId, string userId, CancellationToken cancellationToken = default)
    {
        var auditEvent = new AuditEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            correlationId,
            AuditEventType.ResponseReturned,
            AuditActor.System,
            "auth",
            $"auth://signin/{userId}",
            DateTimeOffset.UtcNow);

        return auditWriter.AppendAsync(auditEvent, cancellationToken);
    }

    public Task PublishConsentUpdateAsync(
        string correlationId,
        string userId,
        string scopeCategory,
        string status,
        CancellationToken cancellationToken = default)
    {
        var auditEvent = new AuditEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            correlationId,
            AuditEventType.PolicyEvaluated,
            AuditActor.System,
            "consent",
            $"consent://{userId}/{scopeCategory}/{status}",
            DateTimeOffset.UtcNow);

        return auditWriter.AppendAsync(auditEvent, cancellationToken);
    }
}
