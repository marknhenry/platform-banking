# Data Model: Phase 0 Trust Foundation

## 1) CustomerIdentitySession

- Purpose: Represents authenticated user context used to gate assistant access.
- Fields:
  - sessionId (UUID, required)
  - userId (string, required)
  - authMethod (enum: entra_oidc, other, required)
  - assuranceLevel (enum: low, medium, high, required)
  - issuedAt (datetime, required)
  - expiresAt (datetime, required)
  - status (enum: active, expired, revoked, required)
- Validation rules:
  - expiresAt > issuedAt
  - active session required before assistant endpoints are served
- State transitions:
  - active -> expired
  - active -> revoked

## 2) ConsentProfile

- Purpose: Source of truth for allowed/denied AI data access and scopes.
- Fields:
  - consentProfileId (UUID, required)
  - userId (string, required)
  - scopeCategory (enum: accounts, transactions, cards, payments, support, required)
  - status (enum: granted, denied, pending, revoked, required)
  - effectiveAt (datetime, required)
  - revokedAt (datetime, optional)
  - propagationDeadlineSeconds (int, required, default 30)
- Validation rules:
  - propagationDeadlineSeconds <= 30
  - revoked status requires revokedAt
- State transitions:
  - pending -> granted | denied
  - granted -> revoked
  - denied -> granted

## 3) TrustIndicatorState

- Purpose: User-visible trust summary displayed in React UI.
- Fields:
  - userId (string, required)
  - readableScopes (array<scopeCategory>, required)
  - blockedScopes (array<scopeCategory>, required)
  - lastConsentUpdateAt (datetime, required)
  - readOnlyMode (boolean, required, fixed true for Phase 0)
- Validation rules:
  - readOnlyMode must be true in Phase 0

## 4) ConversationContext

- Purpose: Maintains continuity for assistant exchanges.
- Fields:
  - conversationId (UUID, required)
  - userId (string, required)
  - startedAt (datetime, required)
  - lastMessageAt (datetime, required)
  - channel (enum: web, mobile, voice, support, required)
  - status (enum: open, escalated, closed, required)
- Validation rules:
  - lastMessageAt >= startedAt

## 5) AgentCapability

- Purpose: Declares agent scope and allowed tools.
- Fields:
  - agentName (enum: orchestrator, account, policy, audit, handoff, required)
  - allowedTools (array<string>, required)
  - allowedScopes (array<scopeCategory>, required)
  - executionMode (enum: read_only, required)
- Validation rules:
  - executionMode must be read_only in Phase 0
  - no direct datastore access permission flags

## 6) PolicyDecisionRecord

- Purpose: Stores request classification and safety gate results.
- Fields:
  - decisionId (UUID, required)
  - conversationId (UUID, required)
  - riskLevel (enum: low, medium, high, required)
  - confidenceScore (decimal, required)
  - actionAllowed (boolean, required)
  - decisionReason (string, required)
  - requiresHandoff (boolean, required)
  - decidedAt (datetime, required)
- Validation rules:
  - if confidenceScore < 0.75 then requiresHandoff = true
  - actionAllowed must be false in Phase 0 for state-changing intents

## 7) AuditEvent

- Purpose: Immutable trace record for each AI interaction step.
- Fields:
  - auditEventId (UUID, required)
  - conversationId (UUID, required)
  - eventType (enum: prompt_received, policy_evaluated, tool_called, response_returned, handoff_created, required)
  - actor (enum: user, agent, system, required)
  - payloadRef (string, required)
  - occurredAt (datetime, required)
  - correlationId (string, required)
- Validation rules:
  - correlationId required on all events
  - event stream ordering by occurredAt must be preserved per conversation

## 8) HandoffPackage

- Purpose: Structured payload transferred to human support.
- Fields:
  - handoffId (UUID, required)
  - conversationId (UUID, required)
  - userId (string, required)
  - triggerReason (enum: low_confidence, unsupported_intent, high_risk_request, required)
  - confidenceScore (decimal, optional)
  - summary (string, required)
  - includedAuditEventIds (array<UUID>, required)
  - createdAt (datetime, required)
  - status (enum: queued, delivered, failed, required)
- Validation rules:
  - low_confidence reason requires confidenceScore < 0.75
  - includedAuditEventIds must contain at least one policy_evaluated event
- State transitions:
  - queued -> delivered | failed

## Relationships

- CustomerIdentitySession 1..* ConversationContext (by userId)
- ConsentProfile 1..* TrustIndicatorState snapshots (by userId)
- ConversationContext 1..* PolicyDecisionRecord
- ConversationContext 1..* AuditEvent
- ConversationContext 0..1 HandoffPackage for a given blocked interaction path
- AgentCapability referenced by PolicyDecisionRecord (agentName attribution)
