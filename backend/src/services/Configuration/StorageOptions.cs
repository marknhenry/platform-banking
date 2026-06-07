namespace PlatformBanking.Services.Configuration;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string SqlConnectionString { get; set; } = string.Empty;

    public string RedisConnectionString { get; set; } = string.Empty;

    public int RevocationPropagationDeadlineSeconds { get; set; } = 30;

    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(SqlConnectionString) &&
        !string.IsNullOrWhiteSpace(RedisConnectionString) &&
        RevocationPropagationDeadlineSeconds is > 0 and <= 30;
}
