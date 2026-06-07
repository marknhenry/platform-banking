using PlatformBanking.Models;

namespace PlatformBanking.Policies;

public interface IPolicyClassificationService
{
    PolicyDecisionRecord Classify(
        Guid conversationId,
        string correlationId,
        string prompt,
        string agentName);
}

public sealed class PolicyClassificationService : IPolicyClassificationService
{
    private const decimal HandoffThreshold = 0.75m;

    public PolicyDecisionRecord Classify(
        Guid conversationId,
        string correlationId,
        string prompt,
        string agentName)
    {
        var normalizedPrompt = prompt.ToLowerInvariant();
        var hasHighRiskKeyword = normalizedPrompt.Contains("transfer") ||
                                 normalizedPrompt.Contains("wire") ||
                                 normalizedPrompt.Contains("close account");

        var confidenceScore = hasHighRiskKeyword ? 0.68m : 0.93m;
        var riskLevel = hasHighRiskKeyword ? RiskLevel.High : RiskLevel.Low;
        var actionAllowed = !hasHighRiskKeyword;
        var requiresHandoff = confidenceScore < HandoffThreshold;

        return new PolicyDecisionRecord(
            Guid.NewGuid(),
            conversationId,
            correlationId,
            agentName,
            riskLevel,
            confidenceScore,
            actionAllowed,
            hasHighRiskKeyword
                ? "Prompt contains state-changing intent and is blocked in read-only mode."
                : "Prompt classified as informational and read-only compatible.",
            requiresHandoff,
            DateTimeOffset.UtcNow);
    }
}
