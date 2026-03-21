# Claude Code Instructions for This Repository

This file is the Claude Code-specific instruction entrypoint. Cross-tool equivalents also exist:

- `AGENTS.md` for Codex-style and generic repository-aware agents
- `.github/copilot-instructions.md` for GitHub Copilot
- `prompts/` for local LLM workflows

## 1. Plan First
- Treat any non-trivial task as architectural work.
- Inspect the existing layer before changing it.
- Keep the generated solution minimal, coherent, and production-oriented.

## 2. Clean Architecture Is Mandatory
- Domain must not depend on Application, Infrastructure, or WebApi.
- Application contains use cases, DTOs, interfaces, validation, and orchestration.
- Infrastructure contains EF Core, repositories, Azure clients, and external integrations.
- WebApi is the transport layer and composition root only.
- Do not move business rules into controllers, EF configurations, or Azure adapters.

## 3. .NET Engineering Rules
- Treat `net10.0` as the template's intended primary target and move the solution there when the SDK is available.
- Keep `net9.0` compatibility where practical.
- Use constructor injection.
- Keep nullable reference types enabled.
- Use async/await correctly and pass cancellation tokens through I/O paths.
- Prefer small, explicit abstractions over framework-heavy indirection.
- Use strongly typed configuration bound from `appsettings`, environment variables, and cloud configuration sources.
- Keep generated code minimal and purposeful.

## 4. EF Core Rules
- Use EF Core migrations instead of manual schema changes.
- Keep `DbContext` and entity configurations in Infrastructure.
- Prefer explicit `IEntityTypeConfiguration<T>` mappings.
- Add concurrency control for important aggregates.
- Bound pagination and query size.
- Introduce a repository only when it improves the application layer.

## 5. API Rules
- Keep controllers thin.
- Use DTOs for request and response payloads.
- Use versioned routes for public APIs.
- Return proper HTTP status codes and ProblemDetails for failures.
- Include health checks, OpenAPI, and structured logging.

## 6. Azure Rules
- Keep Azure SDK usage inside Infrastructure.
- Prefer Managed Identity and Entra ID auth patterns.
- Never hardcode secrets.
- Use Key Vault and App Configuration for production configuration and secrets.
- Hide Azure-specific details behind interfaces when the application layer depends on them.

## 7. Testing Rules
- Use xUnit, FluentAssertions, and NSubstitute consistently.
- Add tests for positive and negative scenarios.
- Keep unit tests focused on business behavior.
- Use integration tests for HTTP endpoints, EF Core mappings, migrations, and persistence behavior.
- Prefer SQLite or Testcontainers where the provider behavior matters.

## 8. Verification Before Finish
- Run build and relevant tests before declaring work complete.
- Review warnings, analyzer output, and architectural drift.
- Confirm project references still point inward only.
- Leave README and template guidance in sync with the code.
