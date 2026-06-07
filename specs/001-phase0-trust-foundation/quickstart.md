# Quickstart: Phase 0 Trust Foundation Validation

This guide validates the Phase 0 trust foundation end-to-end in a branch deployment.

## Prerequisites

- Azure CLI authenticated to a non-production subscription
- Branch environment variables configured for this feature
- IaC templates available under `infra/`
- Backend and frontend dependencies restored

## 1) Provision branch environment

```powershell
# Example shape; concrete scripts are defined during implementation tasks
pwsh ./infra/scripts/deploy-branch.ps1 -Branch 001-implement-phase0-prd -Environment nonprod
```

Expected outcome:
- Branch-scoped backend, frontend, data, and observability resources are provisioned.

## 2) Start application services

```powershell
# Local validation mode (optional)
dotnet run --project backend/src/api
npm --prefix frontend install
npm --prefix frontend run dev
```

Expected outcome:
- React UI and backend API are reachable.

## 3) Execute automated tests (mandatory)

```powershell
# Backend
 dotnet test backend/tests/unit
 dotnet test backend/tests/integration

# Frontend
 npm --prefix frontend test

# End-to-end
 npm --prefix frontend run test:e2e
```

Expected outcome:
- All required test suites pass.

## 4) Validate core scenarios

1. Sign in and consent baseline
- Validate secure sign-in before assistant access.
- Revoke a consent scope and verify assistant enforcement in <30 seconds.

2. Explainable read-only summaries
- Submit summary query and verify rationale and data-use explanation.
- Verify no state-changing action execution occurs.

3. Policy and escalation controls
- Submit unsupported/high-risk prompts.
- Verify execution is blocked and handoff is created when confidence <0.75.

4. Audit trail integrity
- Verify prompt, policy, tool, response, and handoff records are linked by correlation ID.

Expected outcome:
- All scenarios satisfy success criteria SC-001 through SC-007.

## 5) Manual branch validation checklist

- Confirm conversational entry is the default authenticated landing path.
- Confirm trust indicator reflects current consent and read-only mode.
- Confirm blocked execution responses are explainable and actionable.
- Confirm handoff package includes sufficient context for support.

## Reference artifacts

- Spec: `specs/001-phase0-trust-foundation/spec.md`
- Plan: `specs/001-phase0-trust-foundation/plan.md`
- Data model: `specs/001-phase0-trust-foundation/data-model.md`
- Contract: `specs/001-phase0-trust-foundation/contracts/phase0-api.yaml`
