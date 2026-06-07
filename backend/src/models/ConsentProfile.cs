namespace PlatformBanking.Models;

public enum ScopeCategory
{
    Accounts,
    Transactions,
    Cards,
    Payments,
    Support
}

public enum ConsentStatus
{
    Granted,
    Denied,
    Pending,
    Revoked
}

public sealed record ConsentScopeView(
    string UserId,
    IReadOnlyCollection<ScopeCategory> GrantedScopes,
    DateTimeOffset? RevokedAt);

public sealed record ConsentProfile(
    Guid ConsentProfileId,
    string UserId,
    ScopeCategory ScopeCategory,
    ConsentStatus Status,
    DateTimeOffset EffectiveAt,
    DateTimeOffset? RevokedAt,
    int PropagationDeadlineSeconds,
    string CorrelationId)
{
    public bool IsValid() =>
        PropagationDeadlineSeconds <= 30 &&
        (Status != ConsentStatus.Revoked || RevokedAt is not null);
}
