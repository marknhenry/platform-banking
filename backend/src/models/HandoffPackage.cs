namespace PlatformBanking.Models;

public enum HandoffTriggerReason
{
    LowConfidence,
    UnsupportedIntent,
    HighRiskRequest
}

public enum HandoffStatus
{
    Queued,
    Delivered,
    Failed
}

public sealed record HandoffPackage(
    Guid HandoffId,
    Guid ConversationId,
    string CorrelationId,
    string UserId,
    HandoffTriggerReason TriggerReason,
    decimal? ConfidenceScore,
    string Summary,
    IReadOnlyCollection<Guid> IncludedAuditEventIds,
    DateTimeOffset CreatedAt,
    HandoffStatus Status)
{
    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(Summary) &&
        IncludedAuditEventIds.Count > 0 &&
        (TriggerReason != HandoffTriggerReason.LowConfidence || ConfidenceScore is < 0.75m);
}
