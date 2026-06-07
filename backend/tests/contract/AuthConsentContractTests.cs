using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PlatformBanking.ContractTests;

public sealed class AuthConsentContractTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthConsentContractTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("x-user-id", "contract-user");
        _client.DefaultRequestHeaders.Add("x-correlation-id", Guid.NewGuid().ToString("N"));
    }

    [Fact]
    public async Task CreateSession_Returns_ContractShape()
    {
        var response = await _client.PostAsJsonAsync("/v1/auth/session", new
        {
            userId = "contract-user",
            authMethod = "entra_oidc"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<SessionDto>();
        Assert.NotNull(payload);
        Assert.Equal("contract-user", payload!.UserId);
        Assert.Equal("active", payload.Status);
        Assert.Equal("medium", payload.AssuranceLevel);
    }

    [Fact]
    public async Task Consent_GetAndUpdate_Returns_ContractShape()
    {
        var profileResponse = await _client.GetAsync("/v1/consents");
        Assert.Equal(HttpStatusCode.OK, profileResponse.StatusCode);

        var profile = await profileResponse.Content.ReadFromJsonAsync<ConsentProfileDto>();
        Assert.NotNull(profile);
        Assert.True(profile!.ReadOnlyMode);
        Assert.NotEmpty(profile.Consents);

        var updateResponse = await _client.PutAsJsonAsync("/v1/consents/accounts", new { status = "granted" });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var update = await updateResponse.Content.ReadFromJsonAsync<ConsentUpdateDto>();
        Assert.NotNull(update);
        Assert.Equal("accounts", update!.ScopeCategory);
        Assert.Equal("granted", update.Status);
    }

    private sealed record SessionDto(string UserId, string AssuranceLevel, string Status);

    private sealed record ConsentProfileDto(bool ReadOnlyMode, List<ConsentItemDto> Consents);

    private sealed record ConsentItemDto(string ScopeCategory, string Status);

    private sealed record ConsentUpdateDto(string ScopeCategory, string Status);
}
