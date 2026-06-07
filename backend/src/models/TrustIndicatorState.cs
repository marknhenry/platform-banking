namespace PlatformBanking.Models;

public sealed record TrustIndicatorState(
    string UserId,
    IReadOnlyCollection<ScopeCategory> ReadableScopes,
    IReadOnlyCollection<ScopeCategory> BlockedScopes,
    DateTimeOffset LastConsentUpdateAt,
    bool ReadOnlyMode,
    string CorrelationId)
{
    public static TrustIndicatorState CreateDefault(string userId, string correlationId) =>
        new(
            userId,
            Array.Empty<ScopeCategory>(),
            Enum.GetValues<ScopeCategory>(),
            DateTimeOffset.UtcNow,
            ReadOnlyMode: true,
            correlationId);
}
