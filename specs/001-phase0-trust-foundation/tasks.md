# Tasks: Phase 0 Trust Foundation

**Input**: Design documents from `/specs/001-phase0-trust-foundation/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Automated tests are MANDATORY for every feature. Include unit, integration, contract, and end-to-end coverage aligned to this feature scope.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish repository structure, baseline toolchains, and CI entry points for Phase 0.

- [ ] T001 Create backend and frontend folder structure per plan in backend/ and frontend/
- [ ] T002 Initialize .NET backend solution and projects in backend/src/ and backend/tests/
- [ ] T003 [P] Initialize React TypeScript app and test setup in frontend/src/ and frontend/tests/
- [ ] T004 [P] Create IaC and deployment script scaffolding in infra/bicep/ and infra/scripts/
- [ ] T005 [P] Add OpenAPI contract source and validation script wiring for contracts/phase0-api.yaml in backend/tests/contract/
- [ ] T006 Configure CI workflow for backend, frontend, contract, and e2e test jobs in .github/workflows/phase0-ci.yml

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Build cross-cutting trust foundation components required by all user stories.

**CRITICAL**: No user story work can begin until this phase is complete.

- [ ] T007 Create core domain models from data model in backend/src/models/CustomerIdentitySession.cs
- [ ] T008 [P] Create consent and trust models in backend/src/models/ConsentProfile.cs and backend/src/models/TrustIndicatorState.cs
- [ ] T009 [P] Create policy/audit/handoff models in backend/src/models/PolicyDecisionRecord.cs, backend/src/models/AuditEvent.cs, and backend/src/models/HandoffPackage.cs
- [ ] T010 Implement Azure SQL and Redis configuration bindings in backend/src/services/Configuration/StorageOptions.cs
- [ ] T011 [P] Implement shared authentication middleware and session context extraction in backend/src/api/Middleware/AuthSessionMiddleware.cs
- [ ] T012 [P] Implement shared correlation ID, structured logging, and error middleware in backend/src/api/Middleware/RequestContextMiddleware.cs
- [ ] T013 Implement agent capability registry and read-only enforcement baseline in backend/src/agents/AgentCapabilityRegistry.cs
- [ ] T014 Implement policy classification service baseline with confidence handling in backend/src/policies/PolicyClassificationService.cs
- [ ] T015 Implement audit writer abstraction and append-only storage adapter in backend/src/services/Audit/AuditWriter.cs
- [ ] T016 [P] Implement frontend API client foundation and auth session bootstrap in frontend/src/services/apiClient.ts
- [ ] T017 [P] Create global React app shell with conversational-first routing in frontend/src/app/App.tsx
- [ ] T018 Create branch deployment IaC modules for backend/frontend/data/observability in infra/bicep/main.bicep
- [ ] T019 Implement branch deployment script and environment parameter loading in infra/scripts/deploy-branch.ps1

**Checkpoint**: Foundation ready. User story implementation can now proceed.

---

## Phase 3: User Story 1 - Secure Access and Consent Baseline (Priority: P1) MVP

**Goal**: Secure sign-in, consent capture/revocation, and trust indicators before assistant interactions.

**Independent Test**: New and returning users can sign in, set consent, revoke consent, and see enforced trust state within 30 seconds.

### Tests for User Story 1 (MANDATORY)

- [ ] T020 [P] [US1] Add contract tests for auth and consent endpoints in backend/tests/contract/AuthConsentContractTests.cs
- [ ] T021 [P] [US1] Add integration tests for sign-in and consent lifecycle in backend/tests/integration/ConsentLifecycleTests.cs
- [ ] T022 [P] [US1] Add frontend unit tests for trust indicator rendering in frontend/tests/unit/trustIndicator.test.tsx
- [ ] T023 [US1] Add e2e test for sign-in + consent + revocation SLA path in frontend/tests/e2e/consent-revocation.spec.ts

### Implementation for User Story 1

- [ ] T024 [P] [US1] Implement auth session endpoint from contract in backend/src/api/Controllers/AuthController.cs
- [ ] T025 [P] [US1] Implement consent read/update endpoints from contract in backend/src/api/Controllers/ConsentController.cs
- [ ] T026 [US1] Implement consent service with revocation propagation deadline (<30s) in backend/src/services/Consent/ConsentService.cs
- [ ] T027 [US1] Implement trust indicator query composition in backend/src/services/Consent/TrustIndicatorService.cs
- [ ] T028 [P] [US1] Implement consent management UI and trust indicator components in frontend/src/components/ConsentPanel.tsx and frontend/src/components/TrustIndicator.tsx
- [ ] T029 [US1] Wire authenticated conversational landing screen with trust state loading in frontend/src/pages/AssistantHomePage.tsx
- [ ] T030 [US1] Add audit event emission for sign-in and consent updates in backend/src/services/Audit/AuthConsentAuditPublisher.cs

**Checkpoint**: User Story 1 is independently functional and testable.

---

## Phase 4: User Story 2 - Explainable Read-Only Financial Understanding (Priority: P2)

**Goal**: Provide explainable read-only assistant summaries using approved account and transaction data.

**Independent Test**: Users can request financial summaries with rationale and data-source transparency; no state-changing execution is allowed.

### Tests for User Story 2 (MANDATORY)

- [ ] T031 [P] [US2] Add contract tests for assistant query endpoint explainability payload in backend/tests/contract/AssistantQueryContractTests.cs
- [ ] T032 [P] [US2] Add integration tests for read-only summary generation and missing-data handling in backend/tests/integration/AssistantSummaryTests.cs
- [ ] T033 [P] [US2] Add frontend unit tests for summary and explanation rendering in frontend/tests/unit/assistantSummary.test.tsx
- [ ] T034 [US2] Add e2e test for conversational summary flow in frontend/tests/e2e/assistant-summary.spec.ts

### Implementation for User Story 2

- [ ] T035 [P] [US2] Implement assistant query endpoint from contract in backend/src/api/Controllers/AssistantController.cs
- [ ] T036 [US2] Implement account and transaction read adapters with consent enforcement in backend/src/services/Accounts/AccountReadService.cs and backend/src/services/Transactions/TransactionReadService.cs
- [ ] T037 [US2] Implement summary orchestration and explainability payload builder in backend/src/agents/OrchestratorAgent.cs
- [ ] T038 [US2] Implement read-only execution blocker for all state-changing intents in backend/src/policies/ExecutionGuardService.cs
- [ ] T039 [P] [US2] Implement conversational summary panel and explanation drill-down UI in frontend/src/components/AssistantSummaryCard.tsx
- [ ] T040 [US2] Wire assistant query client and page state handling in frontend/src/services/assistantClient.ts and frontend/src/pages/AssistantHomePage.tsx
- [ ] T041 [US2] Add audit event emission for query, tool usage, and response delivery in backend/src/services/Audit/AssistantAuditPublisher.cs

**Checkpoint**: User Stories 1 and 2 are independently functional and testable.

---

## Phase 5: User Story 3 - Policy, Audit, and Human Fallback Controls (Priority: P3)

**Goal**: Enforce policy-based blocking, mandatory handoff at confidence <0.75, and full audit trail retrieval.

**Independent Test**: High-risk and low-confidence requests are blocked/escalated correctly and produce complete linked audit records.

### Tests for User Story 3 (MANDATORY)

- [ ] T042 [P] [US3] Add contract tests for audit events and handoff endpoints in backend/tests/contract/AuditHandoffContractTests.cs
- [ ] T043 [P] [US3] Add integration tests for policy decisions and confidence-threshold handoff in backend/tests/integration/PolicyHandoffTests.cs
- [ ] T044 [P] [US3] Add frontend unit tests for blocked-response and handoff UI states in frontend/tests/unit/handoffState.test.tsx
- [ ] T045 [US3] Add e2e test for unsupported/high-risk prompt escalation in frontend/tests/e2e/policy-handoff.spec.ts

### Implementation for User Story 3

- [ ] T046 [P] [US3] Implement audit retrieval endpoint from contract in backend/src/api/Controllers/AuditController.cs
- [ ] T047 [P] [US3] Implement handoff creation endpoint from contract in backend/src/api/Controllers/HandoffController.cs
- [ ] T048 [US3] Implement policy decision pipeline with confidence threshold (<0.75) enforcement in backend/src/policies/PolicyDecisionEngine.cs
- [ ] T049 [US3] Implement handoff package builder and delivery abstraction in backend/src/services/Handoff/HandoffService.cs
- [ ] T050 [US3] Implement audit correlation and event linking service in backend/src/services/Audit/AuditCorrelationService.cs
- [ ] T051 [P] [US3] Implement frontend blocked-action and handoff status UI in frontend/src/components/HandoffBanner.tsx and frontend/src/components/PolicyBlockNotice.tsx
- [ ] T052 [US3] Add support timeline panel consuming audit trail endpoint in frontend/src/components/AuditTimeline.tsx

**Checkpoint**: All user stories are independently functional and testable.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final hardening, branch deployment validation, and release-readiness checks.

- [ ] T053 [P] Create performance and resilience integration tests for consent revocation SLA and policy latency in backend/tests/integration/NonFunctionalGuardrailTests.cs
- [ ] T054 [P] Add security hardening checks for auth, consent, and API surface in backend/tests/integration/SecurityGuardrailTests.cs
- [ ] T055 [P] Update architecture and operations documentation in docs/phase0-trust-foundation.md
- [ ] T056 Run full CI test matrix and fix regressions in .github/workflows/phase0-ci.yml
- [ ] T057 Provision or update dedicated branch deployment using IaC/scripts in infra/scripts/deploy-branch.ps1
- [ ] T058 Complete manual validation against quickstart scenarios and capture evidence in specs/001-phase0-trust-foundation/checklists/validation-evidence.md
- [ ] T059 Run quickstart.md validation end-to-end and record pass/fail notes in specs/001-phase0-trust-foundation/checklists/requirements.md

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup (Phase 1): no dependencies, starts immediately.
- Foundational (Phase 2): depends on Setup completion; blocks all user story work.
- User Stories (Phases 3-5): all depend on Foundational completion.
- Polish (Phase 6): depends on completion of desired user stories.

### User Story Dependencies

- User Story 1 (P1): starts after Foundational and is the MVP slice.
- User Story 2 (P2): starts after Foundational; uses US1 consent/session foundations but remains independently testable.
- User Story 3 (P3): starts after Foundational; validates policy/audit/handoff controls while integrating with prior story outputs.

### Within Each User Story

- Tests are written first and must fail before implementation.
- Contract + integration tests before service/controller completion.
- Backend policy/service implementation before frontend integration.
- Story-specific audit coverage completed before checkpoint.

### Parallel Opportunities

- Phase 1: T003, T004, and T005 can run in parallel after T001/T002.
- Phase 2: T008, T009, T011, T012, T016, and T017 can run in parallel once base project exists.
- US1: T020/T021/T022 can run in parallel; T024/T025/T028 can run in parallel.
- US2: T031/T032/T033 can run in parallel; T035 and T039 can run in parallel.
- US3: T042/T043/T044 can run in parallel; T046/T047/T051 can run in parallel.
- Polish: T053/T054/T055 can run in parallel before deployment validation tasks.

---

## Parallel Example: User Story 1

```bash
# Parallel test authoring
Task: T020 [US1] backend/tests/contract/AuthConsentContractTests.cs
Task: T021 [US1] backend/tests/integration/ConsentLifecycleTests.cs
Task: T022 [US1] frontend/tests/unit/trustIndicator.test.tsx

# Parallel implementation after failing tests
Task: T024 [US1] backend/src/api/Controllers/AuthController.cs
Task: T025 [US1] backend/src/api/Controllers/ConsentController.cs
Task: T028 [US1] frontend/src/components/ConsentPanel.tsx and frontend/src/components/TrustIndicator.tsx
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 Setup.
2. Complete Phase 2 Foundational prerequisites.
3. Complete Phase 3 User Story 1.
4. Validate revocation SLA and trust indicators in branch deployment.
5. Demo MVP before expanding to summaries and escalation.

### Incremental Delivery

1. Foundation: Setup + Foundational.
2. Deliver US1 for secure sign-in/consent baseline.
3. Deliver US2 for explainable read-only summaries.
4. Deliver US3 for policy, audit, and handoff controls.
5. Complete Polish and branch validation gates before merge.

### Parallel Team Strategy

1. Team aligns on Setup/Foundational tasks first.
2. Once Foundational completes:
   - Engineer A: US1 backend + contract tests
   - Engineer B: US2 assistant flow + frontend summary surfaces
   - Engineer C: US3 policy/audit/handoff and validation automation
3. Integrate through shared CI and branch deployment checkpoints.

---

## Notes

- [P] tasks indicate parallelizable work on separate files without unmet dependencies.
- [USx] labels map each task to independently testable user stories.
- Mandatory testing and branch deployment/manual validation tasks are included to satisfy constitution quality gates.
- Avoid cross-story coupling that blocks independent verification.
