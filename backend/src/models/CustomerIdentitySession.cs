namespace PlatformBanking.Models;

public enum AuthMethod
{
    EntraOidc,
    Other
}

public enum AssuranceLevel
{
    Low,
    Medium,
    High
}

public enum SessionStatus
{
    Active,
    Expired,
    Revoked
}

public sealed record AgentSessionView(
    string UserId,
    AssuranceLevel AssuranceLevel,
    SessionStatus Status,
    string CorrelationId);

public sealed record CustomerIdentitySession(
    Guid SessionId,
    string UserId,
    AuthMethod AuthMethod,
    AssuranceLevel AssuranceLevel,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt,
    SessionStatus Status,
    string CorrelationId)
{
    public bool IsActive(DateTimeOffset utcNow) =>
        Status == SessionStatus.Active && ExpiresAt > utcNow;

    public AgentSessionView ToAgentView() =>
        new(UserId, AssuranceLevel, Status, CorrelationId);
}
