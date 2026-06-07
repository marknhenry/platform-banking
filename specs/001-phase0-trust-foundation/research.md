# Phase 0 Research: Trust Foundation

## Decision 1: Backend and agent stack

- Decision: Use .NET 9 + ASP.NET Core for backend APIs and orchestration with Microsoft Agent Framework for agent composition.
- Rationale: Azure AI app guidance defaults to .NET for unspecified language, and Agent Framework provides explicit orchestration patterns with clear policy and tool boundaries.
- Alternatives considered:
  - Python FastAPI + custom orchestration: strong ecosystem, but less aligned with default enterprise stack and selected internal runtime standards.
  - Monolithic single-agent runtime: simpler to start, but weaker separation for auditability and least-privilege agent boundaries.

## Decision 2: Model and AI integration strategy

- Decision: Integrate through Azure AI Foundry-hosted model endpoints with strict prompt/tool routing controls.
- Rationale: Constitution requires Azure-native controls and Foundry-first usage where fit; Foundry supports governed deployment and centralized model management.
- Alternatives considered:
  - Direct third-party model APIs: rejected due to governance and audit control gaps.
  - Fully local model hosting in Phase 0: rejected because it increases operational complexity and distracts from trust-layer validation goals.

## Decision 3: Read-only policy enforcement design

- Decision: Implement a hard execution gate that blocks all state-changing intents in Phase 0, while still producing explainable responses and handoff options.
- Rationale: Spec clarifies strict read-only scope; this reduces risk while enabling validation of trust, explainability, and policy behavior.
- Alternatives considered:
  - Allow non-monetary low-risk actions: rejected for Phase 0 to avoid ambiguity and reduce compliance surface.
  - Per-action approval for execution: rejected because execution itself is out of scope for this phase.

## Decision 4: Confidence and escalation behavior

- Decision: Enforce mandatory human handoff when model confidence score is <0.75 or intent is unsupported.
- Rationale: Clarified requirement gives deterministic safety behavior and objective testability for escalation logic.
- Alternatives considered:
  - Lower threshold (<0.60): rejected due to elevated risk of incorrect autonomous responses.
  - Higher threshold (<0.90): rejected for likely over-escalation and degraded usability in pilot conditions.

## Decision 5: Consent and revocation architecture

- Decision: Persist consent in Azure SQL and propagate revocation using event + cache invalidation to guarantee enforcement in <30 seconds.
- Rationale: Needs durable records plus low-latency distribution across services and agent nodes.
- Alternatives considered:
  - Database-only lookups on each request: rejected due to latency and scalability overhead.
  - Cache-only consent source of truth: rejected due to weak durability and audit traceability.

## Decision 6: Audit trail architecture

- Decision: Write structured audit events for every AI interaction (prompt, policy, tool calls, outcome, handoff) to append-only storage with query index.
- Rationale: Constitution requires complete reviewability; append-only records support forensic and compliance workflows.
- Alternatives considered:
  - Application logs only: rejected because logs are not sufficient as canonical audit records.
  - Full event sourcing for all business state in Phase 0: rejected as unnecessary complexity for trust-foundation scope.

## Decision 7: Deployment topology for branch validation

- Decision: Use branch-scoped deployments with Bicep templates targeting Azure Container Apps (backend), Static Web Apps (frontend), and shared observability resources.
- Rationale: Matches constitution workflow and keeps environments reproducible and reviewable before merge.
- Alternatives considered:
  - Shared long-lived QA environment only: rejected due to branch isolation requirement.
  - Terraform-first for this phase: viable, but Bicep selected for native Azure alignment and faster team onboarding.

## Decision 8: Testing strategy

- Decision: Enforce layered tests: unit, integration, contract, and end-to-end in CI for each branch deployment.
- Rationale: Constitution mandates automated tests for every feature and manual validation in branch environments.
- Alternatives considered:
  - Unit-only early testing: rejected because policy, audit, and handoff behavior requires integration and e2e validation.
  - Manual-only validation: rejected due to non-repeatability and governance risk.
