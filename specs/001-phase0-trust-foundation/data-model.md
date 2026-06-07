# Data Model: Phase 0 Trust Foundation

## Agent Runtime Design Principles

All domain models in this feature operate within a **Microsoft Semantic Kernel / Agent Framework runtime**. The following rules apply to every model below:

- **Serialization**: All models must be JSON-serializable and safe to pass as `KernelArguments` values or agent tool parameters. No circular references; no non-serializable types.
- **Correlation context**: Every model that flows across an agent boundary MUST carry `correlationId` (request-scoped trace ID) and `conversationId` (session-scoped trace ID) to enable end-to-end audit linking.
- **Agent-scoped views**: Agents receive only the fields they are authorized for, per the `AgentCapabilityRegistry` (T013). Full models are stored by services; agents receive projection types.
- **Immutable inputs**: Models passed into agent tool functions are treated as immutable. Agents never mutate shared state directly — they return results that the orchestrator applies through services.
- **Read-only enforcement**: `executionMode = read_only` is a runtime-enforced invariant in Phase 0. Models do not expose mutation entry points to agent runtimes.

---

## 1) CustomerIdentitySession

- Purpose: Represents authenticated user context used to gate assistant access. Injected into agent kernel context at conversation start.
- **Agent runtime usage**: Passed as `KernelArguments["session"]` to all agent plugins. Agents may read `userId`, `assuranceLevel`, and `status`; they must not receive raw token claims.
- Fields:
  - sessionId (UUID, required)
  - userId (string, required)
  - authMethod (enum: entra_oidc, other, required)
  - assuranceLevel (enum: low, medium, high, required)
  - issuedAt (datetime, required)
  - expiresAt (datetime, required)
  - status (enum: active, expired, revoked, required)
  - correlationId (string, required) — request-scoped trace ID for agent tool call linking
- Validation rules:
  - expiresAt > issuedAt
  - active status required before any agent plugin is invoked
  - correlationId must be set before passing to kernel
- State transitions:
  - active -> expired
  - active -> revoked
- **Agent projection** (AgentSessionView): `{ userId, assuranceLevel, status, correlationId }` — only these fields exposed to agent plugins

---

## 2) ConsentProfile

- Purpose: Source of truth for allowed/denied AI data access and scopes. Read by the OrchestratorAgent and ConsentPlugin before any data tool is invoked.
- **Agent runtime usage**: ConsentPlugin reads a user's effective scopes via `KernelFunction` decorated method. Returns `ConsentScopeView` — never the raw profile.
- Fields:
  - consentProfileId (UUID, required)
  - userId (string, required)
  - scopeCategory (enum: accounts, transactions, cards, payments, support, required)
  - status (enum: granted, denied, pending, revoked, required)
  - effectiveAt (datetime, required)
  - revokedAt (datetime, optional)
  - propagationDeadlineSeconds (int, required, default 30)
  - correlationId (string, required) — set on revocation events for audit linking
- Validation rules:
  - propagationDeadlineSeconds <= 30
  - revoked status requires revokedAt
  - correlationId required on all revocation operations
- State transitions:
  - pending -> granted | denied
  - granted -> revoked
  - denied -> granted
- **Agent projection** (ConsentScopeView): `{ userId, grantedScopes: array<scopeCategory>, revokedAt? }` — resolved effective scopes only

---

## 3) TrustIndicatorState

- Purpose: User-visible trust summary displayed in React UI. Composed by the OrchestratorAgent from live consent and session state.
- **Agent runtime usage**: Assembled by `TrustIndicatorPlugin.GetTrustStateAsync()` as a `KernelFunction`. Result is returned directly to the frontend — it is a projection, not a stored entity.
- Fields:
  - userId (string, required)
  - readableScopes (array<scopeCategory>, required)
  - blockedScopes (array<scopeCategory>, required)
  - lastConsentUpdateAt (datetime, required)
  - readOnlyMode (boolean, required, fixed true for Phase 0)
  - correlationId (string, required) — links this snapshot to the originating request trace
- Validation rules:
  - readOnlyMode must be true in Phase 0
  - correlationId must match the active request context

---

## 4) ConversationContext

- Purpose: Maintains continuity for assistant exchanges. Root context object for all agent plugin invocations within a conversation turn.
- **Agent runtime usage**: Stored in kernel memory / context variable as `KernelArguments["conversation"]`. All plugins within a turn receive this as their scoping context.
- Fields:
  - conversationId (UUID, required)
  - userId (string, required)
  - sessionId (UUID, required) — links to active CustomerIdentitySession
  - startedAt (datetime, required)
  - lastMessageAt (datetime, required)
  - channel (enum: web, mobile, voice, support, required)
  - status (enum: open, escalated, closed, required)
  - correlationId (string, required) — per-turn trace ID; rotated each request
- Validation rules:
  - lastMessageAt >= startedAt
  - correlationId must be refreshed on each new turn invocation
  - escalated status requires a linked HandoffPackage

---

## 5) AgentCapability

- Purpose: Declares agent scope, allowed tools, and runtime constraints. Loaded by `AgentCapabilityRegistry` at startup and enforced before any plugin is registered.
- **Agent runtime usage**: The registry gates `KernelPlugin` registration. Plugins not listed in `allowedTools` for an agent are never added to that agent's kernel instance.
- Fields:
  - agentName (enum: orchestrator, account_reader, policy_checker, audit_writer, handoff_dispatcher, required)
  - allowedTools (array<string>, required) — Semantic Kernel `KernelFunction` names this agent may call
  - allowedScopes (array<scopeCategory>, required) — data scopes this agent may read
  - executionMode (enum: read_only, required)
  - pluginAssemblies (array<string>, required) — plugin class names registered to this agent's kernel
- Validation rules:
  - executionMode must be read_only in Phase 0 (enforced at registry initialization)
  - pluginAssemblies must not include any write-capable service adapters
  - no agent may hold allowedScopes beyond what its user's ConsentProfile grants

---

## 6) PolicyDecisionRecord

- Purpose: Stores request classification and safety gate results. Produced by the PolicyCheckerAgent as output of its `ClassifyRequestAsync` kernel function.
- **Agent runtime usage**: PolicyCheckerAgent returns this as its function result. OrchestratorAgent reads `actionAllowed` and `requiresHandoff` to route the conversation.
- Fields:
  - decisionId (UUID, required)
  - conversationId (UUID, required)
  - correlationId (string, required) — must match the active turn's correlationId
  - agentName (string, required) — which agent produced the decision
  - riskLevel (enum: low, medium, high, required)
  - confidenceScore (decimal, required)
  - actionAllowed (boolean, required)
  - decisionReason (string, required)
  - requiresHandoff (boolean, required)
  - decidedAt (datetime, required)
- Validation rules:
  - confidenceScore < 0.75 → requiresHandoff = true (enforced in PolicyDecisionEngine)
  - actionAllowed must be false in Phase 0 for any state-changing intent
  - correlationId must not be null; must match ConversationContext.correlationId for the turn

---

## 7) AuditEvent

- Purpose: Immutable trace record for each AI interaction step. Written by `AuditWriterPlugin` — a kernel function callable only by agents listed in `AgentCapability.allowedTools`.
- **Agent runtime usage**: Agents emit audit events by invoking `AuditPlugin.WriteEventAsync()` as a `KernelFunction`. The function is append-only; no agent plugin can update or delete an event.
- Fields:
  - auditEventId (UUID, required)
  - conversationId (UUID, required)
  - correlationId (string, required) — links to the turn that produced this event
  - eventType (enum: prompt_received, policy_evaluated, tool_called, response_returned, handoff_created, required)
  - actor (enum: user, agent, system, required)
  - agentName (string, required) — name of the agent plugin that emitted the event
  - payloadRef (string, required) — reference to blob storage payload (not inline)
  - occurredAt (datetime, required)
- Validation rules:
  - correlationId required on all events; must match the active turn
  - agentName must be a registered agent in AgentCapabilityRegistry
  - event stream ordering by occurredAt must be preserved per conversationId
  - payloadRef must point to an existing blob (validated async post-write)

---

## 8) HandoffPackage

- Purpose: Structured payload transferred to human support. Produced by `HandoffDispatcherAgent` when OrchestratorAgent triggers escalation.
- **Agent runtime usage**: HandoffDispatcherAgent constructs this as its function return value. It is not a stored entity the agent mutates — the service layer persists it after validation.
- Fields:
  - handoffId (UUID, required)
  - conversationId (UUID, required)
  - correlationId (string, required) — links to the policy decision that triggered the handoff
  - userId (string, required)
  - triggerReason (enum: low_confidence, unsupported_intent, high_risk_request, required)
  - confidenceScore (decimal, optional)
  - summary (string, required) — agent-generated; must not contain raw PII
  - includedAuditEventIds (array<UUID>, required)
  - createdAt (datetime, required)
  - status (enum: queued, delivered, failed, required)
- Validation rules:
  - low_confidence reason requires confidenceScore < 0.75
  - includedAuditEventIds must contain at least one policy_evaluated event
  - summary must be non-empty; PII stripping applied before storage
  - correlationId must match the PolicyDecisionRecord that triggered this handoff
- State transitions:
  - queued -> delivered | failed

---

## Relationships

- CustomerIdentitySession 1..* ConversationContext (by userId; sessionId FK on ConversationContext)
- ConsentProfile 1..* TrustIndicatorState snapshots (by userId; TrustIndicatorState is a projection, not stored)
- ConversationContext 1..* PolicyDecisionRecord (by conversationId + correlationId)
- ConversationContext 1..* AuditEvent (by conversationId + correlationId)
- ConversationContext 0..1 HandoffPackage per blocked interaction (by conversationId)
- AgentCapability governs which KernelPlugins are registered per agent kernel instance
- PolicyDecisionRecord 1..0..1 HandoffPackage (correlationId links trigger decision to handoff)

---

## Agent-to-Model Access Matrix

| Model | orchestrator | account_reader | policy_checker | audit_writer | handoff_dispatcher |
|---|---|---|---|---|---|
| CustomerIdentitySession | AgentSessionView (read) | AgentSessionView (read) | AgentSessionView (read) | correlationId only | correlationId only |
| ConsentProfile | ConsentScopeView (read) | ConsentScopeView (read) | ConsentScopeView (read) | — | — |
| ConversationContext | full (read) | read | read | read | read |
| PolicyDecisionRecord | full (read) | — | write result | read | read |
| AuditEvent | — | — | — | write (append-only) | read |
| HandoffPackage | trigger only | — | — | — | write result |
