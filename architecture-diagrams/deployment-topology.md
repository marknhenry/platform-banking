# Deployment Topology Diagram

## Purpose

This diagram explains how branch-scoped environments are provisioned in Azure and how
traffic flows between runtime services.

## Deployment Walkthrough

1. The deployment script [infra/scripts/deploy-branch.ps1](../infra/scripts/deploy-branch.ps1)
  loads environment-specific values from [infra/env/dev.json](../infra/env/dev.json) or
  [infra/env/test.json](../infra/env/test.json).
2. The script invokes [infra/bicep/main.bicep](../infra/bicep/main.bicep), which composes
  backend, frontend, data, and observability modules.
3. Azure resources are tagged with branch and environment metadata for traceability.
4. The frontend is hosted in Azure Static Web Apps and calls the backend API hosted on
  Azure Container Apps.
5. Backend APIs use SQL/Redis and emit telemetry to Application Insights and Log Analytics.

## Runtime Notes

- This topology is designed for non-production branch validation.
- The module layout supports independent scaling and replacement of each tier.
- Observability is first-class so policy, consent, and trust flows can be audited.

```mermaid
flowchart TB
    subgraph AzureRG[Azure Resource Group - branch scoped]
      SWA[Azure Static Web App\nfrontend]
      ACA[Azure Container App\nbackend API]
      ENV[Container Apps Environment]
      SQL[(Azure SQL Database)]
      Redis[(Azure Cache for Redis)]
      AI[(Application Insights)]
      LAW[(Log Analytics Workspace)]
    end

    User[Browser User] --> SWA
    SWA --> ACA
    ACA --> ENV
    ACA --> SQL
    ACA --> Redis
    ACA --> AI
    AI --> LAW

    Bicep[infra/bicep/main.bicep\n+ modules] --> AzureRG
    Script[infra/scripts/deploy-branch.ps1] --> Bicep
```
