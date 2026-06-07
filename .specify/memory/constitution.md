<!--
Sync Impact Report
- Version change: N/A (template) -> 1.0.0
- Modified principles:
	- Principle 1 -> I. AI-First Conversational Interface
	- Principle 2 -> II. Explainability and User Transparency
	- Principle 3 -> III. Safety, Policy, and Human Control
	- Principle 4 -> IV. Agent Scope, Least Privilege, and Auditability
	- Principle 5 -> V. Azure-Native Delivery and Continuous Quality
- Added sections:
	- Platform and Implementation Constraints
	- Delivery Workflow and Quality Gates
- Removed sections:
	- None
- Templates requiring updates:
	- ✅ updated: .specify/templates/plan-template.md
	- ✅ updated: .specify/templates/spec-template.md
	- ✅ updated: .specify/templates/tasks-template.md
	- ✅ no files present: .specify/templates/commands/*.md
- Follow-up TODOs:
	- None
-->

# Experiential Banking Platform Constitution

## Core Principles

### I. AI-First Conversational Interface
The platform MUST treat AI conversation as the default interface for core banking
jobs, with contextual UI components supporting conversation rather than replacing
it. All net-new customer journeys MUST define a conversational entry path and
MUST preserve equivalent capability in mobile and web channels where applicable.
Rationale: this product differentiates on discoverability and personalization
through natural language instead of menu-first navigation.

### II. Explainability and User Transparency
Every AI-mediated action MUST present a clear preview of intent, data used,
proposed action, and expected outcome before execution. The system MUST provide
action receipts and understandable explanations for recommendations, forecasts,
and risk-related decisions. Rationale: trust in financial systems requires users
and auditors to understand what happened and why.

### III. Safety, Policy, and Human Control
High-risk or irreversible actions MUST require explicit policy checks and user
confirmation prior to execution. The system MUST enforce role-based access,
consent boundaries, and escalation to human support for uncertainty, exceptions,
or sensitive cases. Rationale: regulated financial operations require bounded AI
autonomy and clear human fallback.

### IV. Agent Scope, Least Privilege, and Auditability
Each backend agent MUST operate within a narrowly declared scope and explicit
permissions, and MUST use approved tools/services rather than direct unrestricted
data access. Every prompt, tool invocation, policy decision, and outcome MUST be
recorded in an immutable audit trail suitable for compliance review. Rationale:
agentic systems remain safe and maintainable only when authority is explicit,
bounded, and observable.

### V. Azure-Native Delivery and Continuous Quality
All production workloads MUST run on Azure and use Azure-native security,
identity, and operational controls when feasible. Infrastructure MUST be defined
and promoted through Infrastructure as Code and scripts. Frontend experiences
MUST use React. Automated tests are mandatory for every feature and MUST run in
continuous integration. Rationale: consistent cloud controls, reproducibility,
and verified quality are non-negotiable for a trusted banking platform.

## Platform and Implementation Constraints

- Microsoft Foundry MUST be the default AI platform capability where fit allows.
- Microsoft Agent Framework SHOULD be used when it satisfies requirements;
	when limitations exist, teams MUST document the selected supported
	alternative and rationale.
- Personalization MUST remain controllable and explainable; opaque optimization
	that cannot be justified to users or risk/compliance reviewers is prohibited.
- Data access for AI features MUST follow least privilege and explicit consent,
	with environment-specific secrets and policies managed through Azure controls.
- Feature designs MUST include cross-channel reusability expectations for agent
	capabilities (web, mobile, voice, and support workflows where in scope).

## Delivery Workflow and Quality Gates

1. Every feature or phase slice MUST be developed on its own branch.
2. Every feature branch MUST have a dedicated Azure deployment provisioned via
	 IaC and deployment scripts for manual validation before merge.
3. Backend agent changes and UI changes MUST be delivered together when they
	 implement a single user journey, and tests MUST be authored with the change.
4. Merge approval requires passing automated tests and successful manual
	 validation in the branch deployment.
5. Environment promotion MUST reuse the same IaC-defined configuration with
	 controlled parameterization; ad hoc manual infrastructure drift is forbidden.
6. Constitution compliance MUST be explicitly checked in planning, specification,
	 and task generation artifacts.

## Governance

This constitution overrides conflicting local conventions for this repository.
Amendments MUST include: (a) proposed text changes, (b) rationale tied to product
or regulatory needs, (c) impact analysis for templates/workflows, and (d)
migration tasks for in-flight work.

Versioning policy for this constitution uses semantic versioning:
- MAJOR: incompatible governance changes, principle removals, or principle
	redefinitions that alter mandatory behavior.
- MINOR: new principle or materially expanded mandatory guidance.
- PATCH: clarification, wording, typo, or non-semantic refinements.

Compliance review expectations:
- Every plan MUST pass a constitution gate before research/design proceeds.
- Every spec MUST include explicit checks for safety, explainability,
	auditability, Azure/IaC compliance, and testing expectations.
- Every task list MUST include mandatory automated test work and branch
	deployment/manual validation tasks.
- Pull requests MUST state constitution alignment or justify approved exceptions.

**Version**: 1.0.0 | **Ratified**: 2026-06-07 | **Last Amended**: 2026-06-07
