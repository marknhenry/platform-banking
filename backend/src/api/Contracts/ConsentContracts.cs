namespace PlatformBanking.Api.Contracts;

public sealed record ConsentItemResponse(
    string ScopeCategory,
    string Status,
    DateTimeOffset EffectiveAt);

public sealed record ConsentProfileResponse(
    string UserId,
    bool ReadOnlyMode,
    IReadOnlyList<ConsentItemResponse> Consents);

public sealed record UpdateConsentRequest(string Status);

public sealed record ConsentUpdateResult(
    string ScopeCategory,
    string Status,
    DateTimeOffset EnforcementBy);

public sealed record TrustIndicatorResponse(
    string UserId,
    IReadOnlyList<string> ReadableScopes,
    IReadOnlyList<string> BlockedScopes,
    DateTimeOffset LastConsentUpdateAt,
    bool ReadOnlyMode);
