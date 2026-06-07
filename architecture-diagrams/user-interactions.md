# User Interaction Diagrams

## Purpose

These sequence diagrams provide behavior-level traces for the primary user paths already
implemented in Phase 0 and US1.

## How to Read These Flows

- Each diagram starts at user intent and follows downstream system calls.
- Arrows to backend participants represent API requests; reverse arrows represent API responses.
- Audit and trust-related participants show where compliance and safety controls are applied.

## Operational Expectations

1. Session bootstrap can tolerate missing server session and continue in demo mode.
2. Consent updates must produce enforceable results within the 30-second requirement window.
3. Trust indicator refresh should immediately reflect current consent posture.
4. Conversation UX remains read-only in this phase and does not execute state-changing actions.

## 1) Sign-in Session Bootstrap

```mermaid
sequenceDiagram
    actor U as User
    participant FE as Frontend App Shell
    participant API as Backend API
    participant AUD as AuthConsentAuditPublisher

    U->>FE: Open app
    FE->>API: GET /session (bootstrap)
    alt Session exists
      API-->>FE: 200 CustomerIdentitySession
      FE-->>U: Signed-in context visible
    else No active session
      API-->>FE: 401 Unauthorized
      FE-->>U: Continue as demo-user
    end

    U->>FE: Trigger sign-in action
    FE->>API: POST /v1/auth/session
    API->>AUD: PublishSignIn(correlationId, userId)
    API-->>FE: 201 Session created
    FE-->>U: Authenticated context loaded
```

## 2) Consent Update and Trust Indicator Refresh

```mermaid
sequenceDiagram
    actor U as User
    participant FE as AssistantHomePage
    participant CONS as ConsentController
    participant CS as ConsentService
    participant TS as TrustIndicatorService
    participant AUD as AuthConsentAuditPublisher

    U->>FE: Click revoke/grant on scope
    FE->>CONS: PUT /v1/consents/{scope}
    CONS->>CS: UpdateConsent(userId, scope, status)
    CS-->>CONS: ConsentUpdateResult(enforcementBy <= 30s)
    CONS->>AUD: PublishConsentUpdate(...)
    CONS-->>FE: 200 Updated consent

    FE->>CONS: GET /v1/consents
    CONS->>CS: GetProfile(userId)
    CS-->>CONS: ConsentProfileResponse
    CONS-->>FE: 200 Profile

    FE->>CONS: GET /v1/consents/trust-indicator
    CONS->>TS: Build(userId, profile)
    TS-->>CONS: TrustIndicatorResponse(read-only + scopes)
    CONS-->>FE: 200 Trust state
    FE-->>U: Updated trust indicator shown
```

## 3) Conversational Interaction (Read-only)

```mermaid
sequenceDiagram
    actor U as User
    participant FE as Chat UI
    participant PAGE as AssistantHomePage
    participant API as Backend API

    U->>FE: Type message and send
    FE-->>U: Local user message bubble
    FE-->>U: Local assistant informational reply

    Note over PAGE,API: In US1, consent and trust APIs are integrated and read-only
    PAGE->>API: GET /v1/consents
    PAGE->>API: GET /v1/consents/trust-indicator
    API-->>PAGE: Current consent + trust state
    PAGE-->>U: Trust panel and consent controls remain synchronized
```
