Rentz Intelligence — Engineering Guidelines

1. Project Mission

Rentz Intelligence is an AI-powered property intelligence system designed to demonstrate production-oriented AI engineering.

The system accepts natural-language questions about properties and uses structured database queries, semantic retrieval, embeddings, vector search, and RAG where appropriate to produce grounded answers.

This project is intentionally separate from the main Rentz property-management and marketplace application.

Core objective

Build an interview-worthy system demonstrating:

- Full-stack engineering
- ASP.NET Core backend development
- PostgreSQL database design
- Entity Framework Core
- Natural-language query understanding
- Structured query planning
- Embeddings
- Vector search with pgvector
- Retrieval-Augmented Generation (RAG)
- Hybrid search
- LLM integration
- Grounded answer generation
- AI guardrails
- Testing and evaluation
- Security
- Performance and observability

Do not optimize for simply producing an AI chatbot. Optimize for demonstrating sound engineering decisions.

---

2. Architecture

The repository contains multiple system boundaries.

rentz-intelligence/
│
├── src/                  # Next.js frontend
├── public/               # Frontend static assets
│
├── backend/              # ASP.NET Core backend
│
├── data/                 # Synthetic property data and documents
│
├── docs/                 # Architecture and technical documentation
│
├── AGENTS.md             # Coding-agent instructions
├── CLAUDE.md             # Claude instructions referencing AGENTS.md
└── README.md

High-level system

User
 │
 ▼
Next.js Frontend
 │
 ▼
ASP.NET Core API
 │
 ├── Authentication / Authorization
 │
 ├── Query Understanding
 │
 ├── Query Planning
 │
 ├── Structured Retrieval ──────► PostgreSQL
 │
 ├── Semantic Retrieval ────────► pgvector
 │
 ├── Document Retrieval
 │
 └── Answer Generation ─────────► LLM

The frontend must not directly access the database.

The frontend must communicate with the backend through defined APIs.

The backend owns business logic, data access, retrieval orchestration, and AI orchestration.

---

3. Product AI vs Coding Agents

"AGENTS.md" governs AI coding agents working on this repository.

It does NOT define the behavior of the Rentz Intelligence product AI.

The product AI must have its own:

- prompts
- system instructions
- retrieval logic
- guardrails
- validation
- evaluation
- model configuration

Do not confuse coding-agent instructions with runtime AI behavior.

---

4. Core Engineering Principles

4.1 Correctness over speed

Do not introduce shortcuts merely to make a feature work quickly.

Prefer:

- clear boundaries
- explicit types
- predictable behavior
- testable code
- maintainable abstractions

Avoid unnecessary abstraction when a simpler implementation is sufficient.

4.2 Understand before modifying

Before changing an existing feature:

1. Inspect the relevant files.
2. Understand the data flow.
3. Identify dependencies.
4. Determine the intended architecture.
5. Make the smallest appropriate change.

Do not rewrite working systems unnecessarily.

4.3 Preserve existing contracts

Do not casually change:

- API contracts
- database schemas
- component interfaces
- shared types
- authentication behavior
- retrieval contracts

If a breaking change is necessary, document why.

---

5. Frontend Rules

The frontend uses:

- Next.js
- React
- TypeScript
- Tailwind CSS
- App Router

Frontend responsibilities include:

- user interface
- user interaction
- displaying property results
- displaying AI responses
- loading/error states
- API communication

The frontend must not contain backend business logic.

Do not:

- connect directly to PostgreSQL
- expose database credentials
- expose LLM API keys
- implement authoritative authorization
- duplicate backend business rules

Use TypeScript strictly.

Avoid "any" unless there is a documented technical reason.

Prefer reusable components when reuse provides meaningful value.

Keep components focused.

---

6. Backend Rules

The backend uses:

- ASP.NET Core
- C#
- Entity Framework Core
- PostgreSQL

The backend is responsible for:

- API endpoints
- validation
- authorization
- business logic
- database access
- retrieval orchestration
- AI orchestration
- error handling

Prefer a layered architecture with clear responsibilities.

A typical request should conceptually follow:

Controller
   ↓
Application / Service Layer
   ↓
Domain Logic
   ↓
Repository / EF Core
   ↓
PostgreSQL

Do not place substantial business logic inside controllers.

Controllers should primarily coordinate HTTP requests and responses.

---

7. Database Rules

PostgreSQL is the primary relational database.

Entity Framework Core is the primary ORM.

Database design must prioritize:

- normalization where appropriate
- explicit relationships
- appropriate indexes
- referential integrity
- predictable query performance

Do not store large denormalized blobs of relational property data when a proper relational model is more appropriate.

Do not modify production-oriented schemas without considering migrations.

Database schema changes should be represented through EF Core migrations.

Never hard-code database credentials.

Secrets must never be committed to Git.

---

8. Property Data Model

Property information should distinguish between structured and unstructured information.

Structured information may include:

- property
- location
- price
- bedrooms
- bathrooms
- property type
- amenities
- availability
- metadata

Unstructured information may include:

- property descriptions
- house rules
- lease information
- maintenance documents
- property policies
- neighborhood descriptions
- uploaded property documents

Use structured queries for deterministic constraints.

Use semantic retrieval when meaning and context matter.

Do not use vector search merely because it is available.

---

9. AI Architecture

The Rentz Intelligence AI should follow a retrieval-first architecture.

Conceptually:

User Query
    ↓
Query Understanding
    ↓
Intent / Query Plan
    ↓
Retrieval Strategy
    ├── Structured Search
    ├── Semantic Search
    └── Document Retrieval
    ↓
Relevant Context
    ↓
LLM
    ↓
Validated / Grounded Response

The system should distinguish between:

Structured questions

Example:

«Find two-bedroom apartments under ₹25,000.»

These should primarily use structured database filtering.

Semantic questions

Example:

«Which properties are suitable for a quiet family lifestyle?»

These may require semantic retrieval.

Document questions

Example:

«What are the maintenance rules for Riverside Apartments?»

These should use document retrieval / RAG.

Hybrid questions

Example:

«Find affordable two-bedroom properties with a quiet environment and parking.»

These may combine structured filtering and semantic retrieval.

---

10. Anti-Hallucination Rule

This is a critical system requirement.

The product AI must not invent property-specific facts.

For property-specific claims, the system should rely on retrieved application data or retrieved documents.

If sufficient information cannot be retrieved, the AI should acknowledge the limitation rather than fabricate an answer.

Bad:

«"The apartment has 24/7 security."»

when the retrieved data contains no such information.

Good:

«"I couldn't find information confirming 24/7 security for this property."»

Groundedness is more important than producing an answer for every query.

---

11. RAG Rules

RAG must be implemented as an actual retrieval pipeline, not simply by placing documents into an LLM prompt.

A typical RAG pipeline should consider:

Document
 ↓
Parsing
 ↓
Chunking
 ↓
Metadata
 ↓
Embedding
 ↓
Vector Storage
 ↓
Query Embedding
 ↓
Candidate Retrieval
 ↓
Filtering / Ranking
 ↓
Context Construction
 ↓
LLM

Document chunks should retain useful metadata such as:

- property ID
- document ID
- document type
- source
- chunk position

Metadata filtering should be used when appropriate.

Do not retrieve unrelated properties merely because their embeddings are semantically similar.

---

12. Structured Search vs Vector Search

Do not replace relational filtering with vector search.

Use PostgreSQL for deterministic constraints such as:

- price
- bedrooms
- bathrooms
- location
- property type
- availability
- numeric ranges

Use vector search for semantic concepts such as:

- atmosphere
- suitability
- preferences
- descriptions
- contextual similarity

Hybrid retrieval should combine both when appropriate.

---

13. LLM Rules

LLMs are reasoning/generation components, not sources of truth.

The application database and retrieved documents are authoritative for property information.

Never place secrets directly in prompts or client-side code.

Model configuration must be externalized.

Prompts should be versionable and maintainable.

Avoid unnecessarily huge prompts.

Do not send irrelevant retrieved context to the model.

Prefer structured outputs when the application needs machine-readable model responses.

Validate model-generated structured output before using it.

---

14. Query Understanding

Natural-language queries should eventually be converted into an explicit internal representation.

For example:

{
  "intent": "property_search",
  "filters": {
    "location": "Gangtok",
    "bedrooms": 2,
    "maxRent": 25000
  },
  "semanticQuery": null
}

The exact schema may evolve.

Do not allow the LLM to directly generate arbitrary SQL.

The application should control the available query operations.

Never execute raw model-generated SQL without strict validation and an explicitly designed safety boundary.

---

15. Security

Never commit:

- API keys
- database passwords
- authentication secrets
- LLM credentials
- private tokens

Use environment variables or an appropriate secret-management mechanism.

Validate external input.

Apply authorization on the backend.

Never trust client-provided authorization information.

Do not expose internal database errors directly to users.

AI-generated output must not bypass application authorization.

A user must never receive property information they are not authorized to access merely because the AI retrieved it.

---

16. Error Handling

Errors should be:

- predictable
- actionable
- logged appropriately
- safe for users

Do not silently swallow exceptions.

Do not expose stack traces or secrets to clients.

Distinguish between:

- validation errors
- authentication errors
- authorization errors
- not-found errors
- database failures
- external AI failures
- retrieval failures
- unexpected application failures

The UI should provide useful loading, empty, and error states.

---

17. Testing

Important logic should be testable independently.

Prioritize tests for:

- query parsing
- query planning
- structured filtering
- retrieval
- document chunking
- metadata filtering
- authorization
- AI output validation
- critical API endpoints

AI behavior should eventually include evaluation cases rather than relying exclusively on manual testing.

Do not claim an AI feature is reliable without testing representative queries.

---

18. Performance

Do not prematurely optimize.

First establish correctness.

When performance becomes relevant, investigate:

- database indexes
- query plans
- vector indexes
- retrieval limits
- result ranking
- caching
- API latency
- LLM latency
- unnecessary model calls

Avoid calling an LLM when deterministic application logic can answer the question.

For example:

"Find properties under ₹20,000"

does not require an LLM to perform the actual database filtering.

---

19. Observability

Important AI operations should eventually be observable.

Useful signals include:

- request latency
- database query latency
- retrieval latency
- number of retrieved documents
- model latency
- model errors
- token usage
- retrieval failures
- query classification
- answer validation failures

Do not log sensitive user information unnecessarily.

Do not log secrets or API keys.

---

20. Data

The initial property dataset may use realistic synthetic data.

Synthetic data should be realistic enough to demonstrate:

- filtering
- ranking
- semantic search
- RAG
- property comparison
- location-aware queries
- document retrieval

Do not use fabricated metrics in documentation or resumes.

If a performance number has not been measured, do not claim it.

---

21. Documentation

Important architectural decisions should be documented in "docs/".

Examples:

- architecture decisions
- database design
- retrieval strategy
- RAG design
- AI evaluation
- security decisions
- performance investigations

Documentation should explain trade-offs rather than merely describing implementation.

For significant decisions, document:

Problem
Options considered
Decision
Reason
Trade-offs
Consequences

---

22. Dependencies

Do not add dependencies without a reason.

Before installing a library:

1. Determine whether the functionality already exists.
2. Check whether the dependency is necessary.
3. Prefer well-maintained libraries.
4. Consider security and maintenance.
5. Keep the dependency surface small.

Do not install multiple libraries that solve the same problem without a clear reason.

---

23. Git Discipline

Keep commits focused.

Prefer commits that represent a coherent engineering change.

Examples:

feat: add property query API
feat: add structured property search
feat: add document chunking pipeline
feat: add pgvector retrieval
fix: validate AI query filters
test: add retrieval evaluation cases
docs: document hybrid retrieval architecture

Do not commit:

- secrets
- ".env" files containing credentials
- generated build artifacts
- "node_modules"
- unnecessary temporary files

---

24. Coding Agent Behavior

Coding agents must:

1. Read "AGENTS.md" before making changes.
2. Inspect relevant existing code before modifying it.
3. Explain significant architectural changes.
4. Avoid unnecessary rewrites.
5. Keep changes scoped to the requested task.
6. Run appropriate tests or validation after changes.
7. Report failures instead of hiding them.
8. Never fabricate successful test results.
9. Never invent APIs or library behavior when verification is possible.
10. Ask for clarification when requirements are genuinely ambiguous.

Coding agents should prefer small, reviewable changes.

Do not modify unrelated files merely for formatting or stylistic consistency.

---

25. Things Coding Agents Must NOT Do

Do not:

- rebuild the entire project without approval
- replace working architecture without justification
- introduce arbitrary frameworks
- expose secrets
- bypass authentication
- bypass authorization
- generate arbitrary SQL from user input
- let an LLM become the source of truth
- fabricate property information
- claim unmeasured performance improvements
- delete working functionality to simplify implementation
- modify unrelated features
- commit generated credentials
- silently change API contracts
- silently change database schemas
- add unnecessary dependencies
- disable TypeScript safety to make code compile
- use "any" as a shortcut
- suppress errors without understanding them
- remove tests to make a build pass

---

26. Development Philosophy

Build the simplest correct version first.

Then improve it deliberately.

The preferred progression is:

Correctness
    ↓
Clear architecture
    ↓
Testing
    ↓
Observability
    ↓
Performance
    ↓
Scale

Do not introduce distributed systems, queues, caching layers, agents, or complex orchestration merely because they sound impressive.

Every architectural component must have a reason to exist.

The purpose of Rentz Intelligence is to demonstrate strong engineering judgment, not maximum technological complexity.

---

27. Interview-Readiness

Major implementation decisions should be understandable and explainable by the developer.

When adding a significant feature, consider:

- Why was this architecture chosen?
- What alternatives were considered?
- What are the failure modes?
- How does it scale?
- How is it secured?
- How is it tested?
- What happens when the AI is wrong?
- What happens when retrieval fails?
- What happens when the database is unavailable?
- What happens when the LLM is unavailable?
- What would change at 10x or 100x scale?

The final system should be something the developer can confidently explain in a software-engineering interview.

---

28. Final Rule

When in doubt:

Prefer correctness, explicitness, security, maintainability, and explainability over cleverness.

Rentz Intelligence should be built as a real software system that happens to contain AI—not as an AI demo with a website around it.