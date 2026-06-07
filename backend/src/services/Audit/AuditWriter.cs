using System.Collections.Concurrent;
using PlatformBanking.Models;

namespace PlatformBanking.Services.Audit;

public interface IAuditWriter
{
    Task<AuditEvent> AppendAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditEvent>> GetByConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);
}

public sealed class AppendOnlyInMemoryAuditWriter : IAuditWriter
{
    private readonly ConcurrentDictionary<Guid, ConcurrentQueue<AuditEvent>> _eventStream = new();

    public Task<AuditEvent> AppendAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        var queue = _eventStream.GetOrAdd(auditEvent.ConversationId, _ => new ConcurrentQueue<AuditEvent>());
        queue.Enqueue(auditEvent);
        return Task.FromResult(auditEvent);
    }

    public Task<IReadOnlyList<AuditEvent>> GetByConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        if (!_eventStream.TryGetValue(conversationId, out var queue))
        {
            return Task.FromResult<IReadOnlyList<AuditEvent>>(Array.Empty<AuditEvent>());
        }

        return Task.FromResult<IReadOnlyList<AuditEvent>>(queue.ToArray());
    }
}
