# Solution Context Diagram

## Purpose

This diagram describes the complete Phase 0 trust foundation boundary: browser user,
React frontend, ASP.NET Core backend, and Azure-hosted data/observability services.

## How the System Works

1. The user interacts with a conversational-first frontend shell.
2. Frontend components call the backend through a centralized API client.
3. Backend middleware establishes request context and session state.
4. Controllers coordinate consent, trust indicators, and audit publishing.
5. Service-layer components enforce read-only policy and produce auditable outputs.
6. Data and telemetry services support operational visibility and future scale.

## Responsibilities by Layer

- Frontend: interaction flow, consent controls, trust visibility, and local chat UX state.
- Backend API: authenticated request orchestration and business rule enforcement.
- Service layer: consent lifecycle, trust-state projection, policy evaluation, and audit writes.
- Platform services: SQL/Redis for persistence and App Insights/Log Analytics for observability.

```mermaid
flowchart LR
    User[End User]

    subgraph FE[Frontend - React + Vite]
      AppShell[App Shell]
      AssistantPage[AssistantHomePage]
      ConsentPanel[ConsentPanel]
      TrustIndicator[TrustIndicator]
      ApiClient[apiClient service]
    end

    subgraph BE[Backend - ASP.NET Core API]
      Middleware[RequestContext + AuthSession Middleware]
      AuthCtrl[AuthController]
      ConsentCtrl[ConsentController]
      ConsentSvc[ConsentService]
      TrustSvc[TrustIndicatorService]
      PolicySvc[PolicyClassificationService]
      CapabilityReg[AgentCapabilityRegistry]
      AuditPub[AuthConsentAuditPublisher]
      AuditWriter[AppendOnly Audit Writer]
    end

    subgraph Data[Data/Config]
      SQL[(Azure SQL)]
      Redis[(Azure Redis)]
      AppInsights[(App Insights)]
      LogAnalytics[(Log Analytics)]
    end

    User --> AppShell
    AppShell --> AssistantPage
    AssistantPage --> ConsentPanel
    AssistantPage --> TrustIndicator
    AppShell --> ApiClient

    ApiClient --> Middleware
    Middleware --> AuthCtrl
    Middleware --> ConsentCtrl

    AuthCtrl --> AuditPub
    ConsentCtrl --> ConsentSvc
    ConsentCtrl --> TrustSvc
    ConsentCtrl --> AuditPub

    PolicySvc --> CapabilityReg
    AuditPub --> AuditWriter

    ConsentSvc -. config .-> SQL
    ConsentSvc -. cache .-> Redis
    AuditWriter -. telemetry .-> AppInsights
    AppInsights --> LogAnalytics
```
