# Product Requirements Document: AI-First Experiential Banking

**Working title:** Experiential Banking Platform  
**Status:** Draft  
**Audience:** Product, engineering, design, risk, compliance

## 1. Product summary

Build a new banking experience where the primary user interface is AI instead of menus. Customers should be able to discover, understand, and act on banking capabilities through natural language, with traditional UI surfaces supporting the AI rather than replacing it.

The product should feel like a financial concierge: conversational, personalized, proactive, and capable of executing banking tasks safely and transparently.

## 2. Problem statement

Most digital banking apps expose products and actions as menus, forms, and dashboards. Users must already know what to look for, where to find it, and how to complete it.

This creates friction for:

- everyday banking tasks
- understanding finances
- finding the right product or service
- handling exceptions and support
- planning and decision-making

It also means that the same app/experience is used for everyone.  Reality is, we all use our banks in a different manner.  

## 3. Product vision

Create a banking platform where:

1. users can ask for what they need in plain language and the system can:
   - explain their financial situation
   - surface relevant actions before they ask
   - execute safe transactions
   - guide them through complex decisions
   - escalate sensitive actions when needed
2. The initial view of the app is not a dashboard or a set of menu items for the user to click, but a customizable, fluid interface that shows the most relevant information and actions for that user.

## 4. Product principles

1. **AI is the default interface.** Users should be able to do most things through conversation, while AI can still surface simple menus and dashboards that are context-aware and user-aware.
2. **Every action must be explainable.** The system should show what it understood, what it will do, and why.
3. **Safety over autonomy.** High-risk actions need confirmation, policy checks, and auditability.
4. **Personalization with guardrails.** Recommendations should adapt to the user without becoming opaque.
5. **Human fallback always exists.** Users can reach a person when AI is insufficient.

## 5. Goals

- Make banking personalized through AI
- Make banking discoverable through natural language
- Reduce task completion time for routine banking
- Increase engagement with financial insights and proactive guidance
- Improve support resolution through AI-assisted service
- Create a differentiated banking experience centered on trust and convenience

## 6. Non-goals for v1

- Fully autonomous financial management without user approval
- Unrestricted AI access to all account actions
- Replacing regulated support processes
- Launching every banking product on day one

## 7. Target users

| Persona | Needs |
|---|---|
| Everyday consumer | Fast answers, simple banking actions, clarity on balances and spending |
| Financial planner | Cash flow, goals, insights, alerts, and guidance |
| Household manager | Multi-account visibility, shared finances, and family-oriented support |
| Support-seeking user | Quick issue resolution and guided troubleshooting |

## 8. Core requirements

- Conversational access to accounts, balances, transactions, cards, payments, and support
- AI that can summarize financial status in plain language
- User-approved execution of banking actions
- Personalized insights and alerts
- Transparent reasoning and action previews
- Role-based access and policy controls
- Full audit trail for AI-assisted actions
- Escalation to humans for sensitive or uncertain cases
- Widgets and customizable components need to be visible on the page so the user gets a personalized view based on profile and behavior.

## 9. Tech stack and delivery constraints

- Everything must run on Azure and use Azure-native capabilities and security services where possible.
- Everything must be defined as infrastructure as code, with all configurations deployable to Azure through scripts.
- Testing is mandatory and continuous; every feature should be covered by automated tests.
- Each feature must get its own branch and its own deployment so it can be manually validated before merge.
- Use Microsoft Foundry where possible for AI platform capabilities.
- Use Microsoft Agent Framework when it fits the need; if it has limitations, use the best supported alternative.
- The frontend must use React.

## 10. Delivery workflow

1. Create a feature branch for each feature or phase slice.
2. Provision a dedicated Azure deployment for that branch using IaC and deployment scripts.
3. Implement backend agents and UI changes together, with tests written alongside the feature.
4. Validate the feature in its branch deployment before merge.
5. Merge only after the feature passes manual validation and automated tests.
6. Promote the same IaC-defined configuration through environments.

## 11. Agentic backend model

The backend should be agentic: user requests are interpreted by specialized agents that can read approved data, call backend services, and initiate actions within policy boundaries.

The platform should include an orchestrator plus task-specific agents. Each agent must have a narrow scope, explicit permissions, full logging, and a clear fallback path.

**Core agent types**

- **Orchestrator agent** - routes user requests to the right specialist, manages conversation state, and decides when to escalate.
- **Account agent** - reads balances, transactions, account status, and account metadata.
- **Payments agent** - initiates transfers, bill pay, and payment status checks.
- **Card agent** - handles card controls such as lock/unlock, replacement, and limit checks.
- **Insights agent** - produces summaries, forecasts, anomaly detection, and personalized guidance.
- **Support agent** - handles service workflows, disputes, document collection, and case summaries.
- **Policy agent** - checks permissions, risk thresholds, and required confirmations before action.
- **Audit agent** - records prompts, tool calls, decisions, and outcomes for review.
- **Handoff agent** - packages context for human support when the system cannot safely proceed.

**Agent requirements**

- Agents must not operate outside their declared scope.
- High-risk actions must require policy approval or user confirmation.
- Every agent action must produce an audit trail.
- Agents should be reusable across web, mobile, voice, and support channels.
- Agents should expose backend capabilities through tools rather than direct database access.

## 12. Phase plan

Each phase should deliver two coordinated streams of work:

1. **Agentic backend stream** - expand backend agents, policies, orchestration, and tool access.
2. **AI-assisted UI stream** - expand conversational UI, contextual surfaces, personalization, and action review.

### Phase 0 - Foundation and trust layer

**Objective:** establish the safety, data, and AI platform required for everything else.

**Agentic backend stream**

- Identity, authentication, and consent flows
- Banking data aggregation layer
- Account, transaction, card, and payment domain models
- AI orchestration layer and tool routing
- Audit logging for prompts, decisions, and actions
- Permissioning and policy engine
- Agent registry and orchestration layer
- Task-specific backend agents
- Human handoff path
- Observability, analytics, and feedback capture

**AI-assisted UI stream**

- Conversational entry point and assistant shell
- Personalized home view with relevant cards, summaries, and actions
- Approval and consent surfaces for sensitive actions
- Conversation history and action receipts
- Trust indicators showing what the AI can access and do

**Exit criteria**

- Users can securely sign in
- AI can read approved data sources
- Every AI action is logged and reviewable

### Phase 1 - Conversational banking core

**Objective:** let users ask questions and complete basic banking tasks through AI.

**Agentic backend stream**

- Conversational balance and transaction lookup
- Natural-language spending search
- Cash flow and account summaries
- Transfers between owned accounts
- Bill payment initiation
- Card controls: lock/unlock, limit checks, replacement requests
- Secure confirmation flows for sensitive actions
- Conversation history and action receipts

**AI-assisted UI stream**

- Chat-first task flows with structured action previews
- Contextual account cards for balances, spending, and transfers
- Inline confirmations for high-impact actions
- Search and filter UI that mirrors conversational intent
- Persistent conversation state across sessions and devices

**Exit criteria**

- Users can complete common banking tasks without navigating menus
- AI can explain each action before execution

### Phase 2 - Financial intelligence and guidance

**Objective:** make the product proactive, not just reactive.

**Agentic backend stream**

- Budget creation and category analysis
- Spending anomaly detection
- Forecasting for balances and bills
- Goal tracking and savings nudges
- Personalized recommendations
- “What changed?” explanations for spending and income
- Proactive alerts for low balance, upcoming bills, and unusual activity

**AI-assisted UI stream**

- Insight cards surfaced on the home view
- Explainable spending breakdowns and trend views
- Goal progress visualizations
- Alert inbox with AI-generated summaries
- Personalized nudges and recommended next actions

**Exit criteria**

- Users receive useful guidance before they ask for it
- Insights are understandable and actionable

### Phase 3 - Assisted support and service operations

**Objective:** use AI to reduce friction in customer service and complex workflows.

**Agentic backend stream**

- AI support assistant for common issues
- Guided dispute and chargeback workflows
- Document upload and extraction for banking requests
- KYC / re-verification support
- Status tracking for pending requests
- Human escalation with conversation context transfer
- Case summaries for support agents

**AI-assisted UI stream**

- Support chat with guided next steps
- Document capture and upload screens
- Case timeline and request status views
- Escalation handoff screens for users and agents
- Contextual help embedded in task screens

**Exit criteria**

- AI resolves a meaningful share of support requests
- Human agents receive complete context when escalation happens

### Phase 4 - Product expansion and ecosystem intelligence

**Objective:** extend the experience to deeper banking products and external workflows.

**Agentic backend stream**

- Loan and credit product discovery through AI
- Pre-qualification and eligibility guidance
- Merchant offers and rewards discovery
- Retail lending and credit workflows
- Recurring payment and subscription intelligence
- External account connectivity
- Voice and multimodal interaction
- Multilingual support

**AI-assisted UI stream**

- Product recommendation surfaces tied to user intent
- Offer and eligibility preview cards
- Multimodal interaction patterns for voice, text, and image inputs
- Personalized consumer finance dashboard views
- Localization-aware UI surfaces

**Exit criteria**

- AI can guide users to the right product or workflow
- The platform supports broader financial engagement beyond checking and savings

### Phase 5 - Agentic automation and personalization at scale

**Objective:** add controlled automation for repetitive financial tasks.

**Agentic backend stream**

- User-defined rules and automations
- Scheduled actions with approval thresholds
- Household or shared financial views
- Adaptive personalization based on behavior and preferences
- AI-generated monthly financial review
- Autonomous monitoring with exception handling
- Policy-based execution for low-risk actions

**AI-assisted UI stream**

- Automation setup wizard and rule builder
- Controls for approvals, thresholds, and schedules
- Household and shared-view dashboards
- Personalization settings and preference controls
- Review surfaces for autonomous actions and exceptions

**Exit criteria**

- Users can delegate repetitive tasks safely
- Automation remains visible, reversible, and bounded

## 13. Key feature backlog by phase

| Phase | Feature group | Notes |
|---|---|---|
| 0 | AI platform foundation | Identity, policy, audit, orchestration |
| 1 | Conversational core banking | Ask, search, transfer, pay, card controls |
| 2 | Financial intelligence | Forecasting, budgeting, nudges, insights |
| 3 | Service and ops | Support, disputes, documents, escalation |
| 4 | Product expansion | Lending, rewards, multimodal |
| 5 | Controlled automation | Rules, schedules, delegated actions |

## 14. Success metrics

- Percentage of core tasks completed through AI
- Time to complete common banking tasks
- AI containment and escalation accuracy
- Support resolution time
- Feature adoption for insights and alerts
- User trust / satisfaction scores
- Reduction in task abandonment

## 15. Risks and constraints

- Regulatory and compliance requirements may limit AI autonomy
- Hallucinations or incorrect tool use must be prevented
- Sensitive financial data needs strong access control and logging
- AI decisions must be explainable to users and auditors
- Human fallback is required for uncertain or high-risk actions

## 16. Open questions

- What banking products are in scope for launch?
- Which actions can AI execute without human review?
- What is the trust policy for confirmations and reversals?
- Which markets and regulations apply at launch?
- What degree of voice or multimodal support is required in v1?

## 17. Recommended next step

Convert this draft into a release plan by choosing:

1. target customer segment
2. launch geography
3. first banking products
4. AI autonomy boundaries
5. compliance and support operating model
