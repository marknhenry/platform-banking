using PlatformBanking.Api.Contracts;
using PlatformBanking.Models;

namespace PlatformBanking.Services.Consent;

public interface ITrustIndicatorService
{
    TrustIndicatorResponse Build(string userId, ConsentProfileResponse profile);
}

public sealed class TrustIndicatorService : ITrustIndicatorService
{
    public TrustIndicatorResponse Build(string userId, ConsentProfileResponse profile)
    {
        var readable = profile.Consents
            .Where(c => string.Equals(c.Status, ConsentStatus.Granted.ToString(), StringComparison.OrdinalIgnoreCase))
            .Select(c => c.ScopeCategory)
            .ToList();

        var blocked = profile.Consents
            .Where(c => !string.Equals(c.Status, ConsentStatus.Granted.ToString(), StringComparison.OrdinalIgnoreCase))
            .Select(c => c.ScopeCategory)
            .ToList();

        return new TrustIndicatorResponse(
            userId,
            readable,
            blocked,
            DateTimeOffset.UtcNow,
            ReadOnlyMode: true);
    }
}
