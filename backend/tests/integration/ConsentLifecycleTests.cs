using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PlatformBanking.IntegrationTests;

public sealed class ConsentLifecycleTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ConsentLifecycleTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("x-user-id", "integration-user");
        _client.DefaultRequestHeaders.Add("x-correlation-id", Guid.NewGuid().ToString("N"));
    }

    [Fact]
    public async Task User_Can_Set_And_Revoke_Consent_With_Enforcement_Window()
    {
        var grant = await _client.PutAsJsonAsync("/v1/consents/transactions", new { status = "granted" });
        Assert.Equal(HttpStatusCode.OK, grant.StatusCode);

        var revoke = await _client.PutAsJsonAsync("/v1/consents/transactions", new { status = "revoked" });
        Assert.Equal(HttpStatusCode.OK, revoke.StatusCode);

        var revokePayload = await revoke.Content.ReadFromJsonAsync<ConsentUpdateDto>();
        Assert.NotNull(revokePayload);

        var enforcementWindow = revokePayload!.EnforcementBy - DateTimeOffset.UtcNow;
        Assert.True(enforcementWindow.TotalSeconds <= 30.1);
    }

    [Fact]
    public async Task TrustIndicator_Reflects_Granted_And_Blocked_Scopes()
    {
        await _client.PutAsJsonAsync("/v1/consents/accounts", new { status = "granted" });
        await _client.PutAsJsonAsync("/v1/consents/cards", new { status = "revoked" });

        var response = await _client.GetAsync("/v1/consents/trust-indicator");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var indicator = await response.Content.ReadFromJsonAsync<TrustIndicatorDto>();
        Assert.NotNull(indicator);
        Assert.True(indicator!.ReadOnlyMode);
        Assert.Contains("accounts", indicator.ReadableScopes);
        Assert.Contains("cards", indicator.BlockedScopes);
    }

    private sealed record ConsentUpdateDto(DateTimeOffset EnforcementBy);

    private sealed record TrustIndicatorDto(
        bool ReadOnlyMode,
        List<string> ReadableScopes,
        List<string> BlockedScopes);
}
