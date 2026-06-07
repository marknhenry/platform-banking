namespace PlatformBanking.Models;

public enum AuditEventType
{
    PromptReceived,
    PolicyEvaluated,
    ToolCalled,
    ResponseReturned,
    HandoffCreated
}

public enum AuditActor
{
    User,
    Agent,
    System
}

public sealed record AuditEvent(
    Guid AuditEventId,
    Guid ConversationId,
    string CorrelationId,
    AuditEventType EventType,
    AuditActor Actor,
    string AgentName,
    string PayloadRef,
    DateTimeOffset OccurredAt);
