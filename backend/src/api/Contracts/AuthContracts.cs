namespace PlatformBanking.Api.Contracts;

public sealed record CreateSessionRequest(string UserId, string AuthMethod);

public sealed record CustomerIdentitySessionResponse(
    Guid SessionId,
    string UserId,
    string AssuranceLevel,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt,
    string Status);
