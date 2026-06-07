# End-to-End Walkthrough

## Scope

This document provides a narrative walkthrough of how the current Platform Banking
solution works across frontend, backend, and infrastructure boundaries.

## 1. Application Startup

1. The browser loads the frontend shell hosted in Azure Static Web Apps.
2. React mounts through `main.tsx`, then delegates to the app shell in `src/app/App.tsx`.
3. The app shell initializes theme and hash-based navigation state.
4. The shell attempts session bootstrap using `GET /session`.
5. If no session exists, the UI continues in demo mode and still exposes trust/consent controls.

## 2. Request Context and Session Handling

1. Every backend request passes through request-context middleware.
2. Correlation IDs are propagated or generated and attached to request/response.
3. Authentication middleware extracts lightweight user/session headers into request context.
4. Controllers consume that context to resolve user identity for business operations.

## 3. Consent and Trust Flow

1. `AssistantHomePage` loads consent profile via `GET /v1/consents`.
2. The same page loads trust posture via `GET /v1/consents/trust-indicator`.
3. User updates consent from the `ConsentPanel` using `PUT /v1/consents/{scope}`.
4. Backend `ConsentService` updates state and returns an `enforcementBy` timestamp.
5. Backend `TrustIndicatorService` rebuilds user-facing trust state from current consent data.
6. Frontend refreshes both panels to ensure consistency after each update.

## 4. Audit and Safety Controls

1. Auth and consent operations publish audit events through `AuthConsentAuditPublisher`.
2. Events are written append-only by `AppendOnlyInMemoryAuditWriter` in current baseline.
3. Capability registry and policy baseline enforce read-only constraints for this phase.
4. Correlation IDs connect UI actions to backend processing and audit traces.

## 5. Deployment and Runtime Topology

1. Branch deployment script loads environment settings and invokes Bicep entry point.
2. Bicep modules deploy frontend, backend, data, and observability resources.
3. Frontend calls backend APIs; backend integrates with SQL/Redis and telemetry services.
4. App Insights and Log Analytics provide operational diagnostics and traceability.

## 6. Validation Story

Current validation confirms:

1. Backend unit, integration, and contract tests pass.
2. Frontend unit and e2e tests pass.
3. OpenAPI contract validation passes.

This means the implemented architecture is executable, testable, and aligned with the
Phase 0 trust foundation direction.
