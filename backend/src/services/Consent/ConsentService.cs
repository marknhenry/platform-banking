using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using PlatformBanking.Api.Contracts;
using PlatformBanking.Models;
using PlatformBanking.Services.Configuration;

namespace PlatformBanking.Services.Consent;

public interface IConsentService
{
    ConsentProfileResponse GetProfile(string userId);

    ConsentUpdateResult UpdateConsent(string userId, string scopeCategory, string status);
}

public sealed class ConsentService(IOptions<StorageOptions> storageOptions) : IConsentService
{
    private readonly StorageOptions _storageOptions = storageOptions.Value;
    private readonly ConcurrentDictionary<string, Dictionary<ScopeCategory, ConsentStatus>> _store = new();

    public ConsentProfileResponse GetProfile(string userId)
    {
        var userConsents = _store.GetOrAdd(userId, _ => CreateDefaultConsents());
        var items = userConsents
            .Select(entry => new ConsentItemResponse(
                entry.Key.ToString().ToLowerInvariant(),
                entry.Value.ToString().ToLowerInvariant(),
                DateTimeOffset.UtcNow))
            .ToList();

        return new ConsentProfileResponse(userId, true, items);
    }

    public ConsentUpdateResult UpdateConsent(string userId, string scopeCategory, string status)
    {
        if (!Enum.TryParse<ScopeCategory>(scopeCategory, true, out var scope))
        {
            throw new InvalidOperationException($"Unsupported scope category '{scopeCategory}'.");
        }

        if (!Enum.TryParse<ConsentStatus>(status, true, out var consentStatus))
        {
            throw new InvalidOperationException($"Unsupported consent status '{status}'.");
        }

        var userConsents = _store.GetOrAdd(userId, _ => CreateDefaultConsents());
        userConsents[scope] = consentStatus;

        return new ConsentUpdateResult(
            scope.ToString().ToLowerInvariant(),
            consentStatus.ToString().ToLowerInvariant(),
            DateTimeOffset.UtcNow.AddSeconds(_storageOptions.RevocationPropagationDeadlineSeconds));
    }

    private static Dictionary<ScopeCategory, ConsentStatus> CreateDefaultConsents() =>
        Enum.GetValues<ScopeCategory>().ToDictionary(scope => scope, _ => ConsentStatus.Pending);
}
