using PlatformBanking.Models;

namespace PlatformBanking.Agents;

public sealed record AgentCapability(
    string AgentName,
    IReadOnlyCollection<string> AllowedTools,
    IReadOnlyCollection<ScopeCategory> AllowedScopes,
    string ExecutionMode,
    IReadOnlyCollection<string> PluginAssemblies);

public interface IAgentCapabilityRegistry
{
    AgentCapability GetOrThrow(string agentName);

    bool CanUseTool(string agentName, string toolName);
}

public sealed class AgentCapabilityRegistry : IAgentCapabilityRegistry
{
    private const string ReadOnlyMode = "read_only";

    private readonly Dictionary<string, AgentCapability> _capabilities;

    public AgentCapabilityRegistry(IEnumerable<AgentCapability> capabilities)
    {
        _capabilities = capabilities.ToDictionary(c => c.AgentName, StringComparer.OrdinalIgnoreCase);
        EnsureReadOnlyBaseline();
    }

    public AgentCapability GetOrThrow(string agentName)
    {
        if (_capabilities.TryGetValue(agentName, out var capability))
        {
            return capability;
        }

        throw new InvalidOperationException($"Agent capability is not registered for '{agentName}'.");
    }

    public bool CanUseTool(string agentName, string toolName)
    {
        var capability = GetOrThrow(agentName);
        return capability.AllowedTools.Contains(toolName, StringComparer.OrdinalIgnoreCase);
    }

    private void EnsureReadOnlyBaseline()
    {
        foreach (var capability in _capabilities.Values)
        {
            if (!string.Equals(capability.ExecutionMode, ReadOnlyMode, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Agent '{capability.AgentName}' is not configured with read-only execution mode.");
            }
        }
    }

    public static IReadOnlyCollection<AgentCapability> CreateDefault() =>
    [
        new AgentCapability(
            "orchestrator",
            ["classify_request", "write_audit_event", "create_handoff"],
            [ScopeCategory.Accounts, ScopeCategory.Transactions, ScopeCategory.Support],
            ReadOnlyMode,
            ["OrchestratorPlugin"]),
        new AgentCapability(
            "policy_checker",
            ["classify_request"],
            [ScopeCategory.Support],
            ReadOnlyMode,
            ["PolicyPlugin"]),
        new AgentCapability(
            "audit_writer",
            ["write_audit_event"],
            [ScopeCategory.Support],
            ReadOnlyMode,
            ["AuditPlugin"])
    ];
}
