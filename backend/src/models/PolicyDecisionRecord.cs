namespace PlatformBanking.Models;

public enum RiskLevel
{
    Low,
    Medium,
    High
}

public sealed record PolicyDecisionRecord(
    Guid DecisionId,
    Guid ConversationId,
    string CorrelationId,
    string AgentName,
    RiskLevel RiskLevel,
    decimal ConfidenceScore,
    bool ActionAllowed,
    string DecisionReason,
    bool RequiresHandoff,
    DateTimeOffset DecidedAt);
