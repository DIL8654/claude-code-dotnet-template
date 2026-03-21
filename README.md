# Claude Code Template for ASP.NET Core and .NET

This repository is a starter kit for modern C#, ASP.NET Core, Clean Architecture, and EF Core. It began as a Claude Code-oriented template, but it is now structured to work across Claude Code, Codex-style agents, GitHub Copilot, and local LLM workflows as well.

The template is intentionally small. It gives both humans and AI tools a strong set of repo rules, prompt assets, starter projects, and engineering conventions without turning the repository into a framework.

## What This Template Enforces

- Clean Architecture with inward-only dependencies
- ASP.NET Core Web API as the transport layer
- EF Core in Infrastructure, with migrations from day one
- Strongly typed configuration
- Structured logging, health checks, and OpenAPI
- xUnit-based testing with positive and negative scenarios
- Azure-friendly integration guidance with Managed Identity as the default
- Cross-tool AI prompt compatibility

## Repository Layout

```text
.
├── .github
│   └── copilot-instructions.md
├── .claude
│   ├── agents
│   │   ├── azure-engineer.md
│   │   ├── code-reviewer.md
│   │   ├── csharp-architect.md
│   │   ├── devops-engineer.md
│   │   ├── dotnet-engineer.md
│   │   ├── ef-core-engineer.md
│   │   ├── security-engineer.md
│   │   └── test-automator.md
│   └── skills
│       ├── README.md
│       ├── api-contract-review
│       ├── architecture-review
│       ├── azure-integration
│       ├── clean-code
│       ├── design-patterns
│       ├── ef-core-patterns
│       ├── logging-patterns
│       ├── security-audit
│       └── testing-patterns
├── AGENTS.md
├── CLAUDE.md
├── Directory.Build.props
├── Directory.Packages.props
├── ClaudeCodeDotNetTemplate.sln
├── prompts
│   ├── README.md
│   ├── system-dotnet-template.md
│   ├── use-a-skill.md
│   └── use-an-agent.md
├── src
│   ├── Application
│   ├── Domain
│   ├── Infrastructure
│   └── WebApi
└── tests
    ├── IntegrationTests
    └── UnitTests
```

## Solution Structure

### `src/Domain`
Pure domain model. No dependencies on Infrastructure or WebApi.

### `src/Application`
Use cases, DTOs, interfaces, validation, orchestration, and application-facing abstractions.

### `src/Infrastructure`
EF Core `DbContext`, entity configuration, repositories, external integrations, and runtime implementations.

### `src/WebApi`
Composition root, controllers, middleware, configuration, and HTTP concerns.

### `tests`
Unit and integration test projects with starter patterns for service and endpoint verification.

### `AGENTS.md`
Cross-tool repository rules for Codex-style agents and any tool that looks for a general agent instruction file.

### `.github/copilot-instructions.md`
GitHub Copilot-specific repository instructions.

### `.claude`
Detailed prompt library with specialized agents and focused skills.

### `prompts`
Portable prompt entrypoints for local LLM tools and generic workflows.

## SDK and Target Framework Strategy

The repository is designed for `.NET 10` first. The latest stable SDK available in this environment is `.NET 9.0.102`, so the project files currently build as `net9.0` while documenting `net10.0` as the intended primary target for future template use.

When you move this starter onto a machine with the .NET 10 SDK installed, update the shared target framework setting in `Directory.Build.props` and, if useful for your organization, multi-target `net10.0;net9.0`.

## Getting Started

1. Update the solution and namespace names to match your service.
2. Set the `ConnectionStrings__AppDb` value for your environment.
3. Restore, build, and test:

```bash
dotnet restore
dotnet build
dotnet test
```

4. Create the first migration:

```bash
dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/WebApi --output-dir Persistence/Migrations
dotnet ef database update --project src/Infrastructure --startup-project src/WebApi
```

## Cross-Tool AI Compatibility

This repository uses several instruction layers so different tools can consume the same engineering conventions.

| Tool | Primary Entry Point | Notes |
|------|---------------------|-------|
| Claude Code | `CLAUDE.md` and `.claude/` | Best experience with specialized agents and skills |
| Codex / repository-aware coding agents | `AGENTS.md` | Use `.claude/` files as deeper task-specific prompt assets |
| GitHub Copilot | `.github/copilot-instructions.md` | Best for Copilot Chat and repo-scoped Copilot guidance |
| Local LLM tools | `prompts/README.md` and `prompts/*.md` | Load prompt files explicitly |

### Recommended Cross-Tool Strategy

Use the files in layers:

1. Start with repository-wide rules
   Files: `AGENTS.md`, `CLAUDE.md`, or `.github/copilot-instructions.md`
2. Load a specialized agent when the task needs a role
   Examples: `.claude/agents/dotnet-engineer.md`, `.claude/agents/ef-core-engineer.md`
3. Load a skill when the task needs a rubric or focused checklist
   Examples: `.claude/skills/api-contract-review/SKILL.md`, `.claude/skills/testing-patterns/SKILL.md`

This keeps the prompt stack small while still giving the model the right context.

## How to Use This Template With Claude Code

Claude Code can work directly with:

- `CLAUDE.md`
- `.claude/agents/*`
- `.claude/skills/*`

Example workflow:

1. Ask Claude Code to read `CLAUDE.md`.
2. Reference one specialized agent if needed.
3. Reference one skill if the task needs a checklist or rubric.

Example prompt:

```text
Read CLAUDE.md, .claude/agents/dotnet-engineer.md, and .claude/skills/testing-patterns/SKILL.md.
Use them while implementing this API feature and its tests.
```

## How to Use This Template With Codex or Other Repository-Aware Agents

Use:

- `AGENTS.md` as the main instruction file
- `.claude/agents/*` for specialized roles
- `.claude/skills/*/SKILL.md` for focused guidance

Example prompt:

```text
Read AGENTS.md, .claude/agents/ef-core-engineer.md, and .claude/skills/ef-core-patterns/SKILL.md.
Use them while reviewing and updating the persistence layer.
```

## How to Use This Template With GitHub Copilot

GitHub Copilot does not natively use the `.claude` folder convention, so this repository includes:

- `.github/copilot-instructions.md`

Recommended usage:

1. Keep `.github/copilot-instructions.md` in the repository root’s `.github` folder.
2. Use Copilot Chat with repo context enabled.
3. If a task is specialized, paste or reference the relevant `.claude/agents/*.md` or `.claude/skills/*/SKILL.md` content into the chat.

Example Copilot Chat prompt:

```text
Follow the repository instructions in .github/copilot-instructions.md.
Also use .claude/skills/api-contract-review/SKILL.md as the review rubric for these controller changes.
```

## How to Use This Template With Local LLMs

For local agents, CLI wrappers, or prompt-driven editors:

1. Start with `prompts/system-dotnet-template.md` or `AGENTS.md`.
2. Add one specialized agent from `.claude/agents/` if needed.
3. Add one skill from `.claude/skills/` if needed.
4. Keep the prompt set small and task-specific.

Useful files:

- `prompts/README.md`
- `prompts/system-dotnet-template.md`
- `prompts/use-an-agent.md`
- `prompts/use-a-skill.md`

Example local prompt:

```text
Read AGENTS.md, prompts/use-an-agent.md, .claude/agents/security-engineer.md, and .claude/skills/security-audit/SKILL.md.
Use them while reviewing this API and configuration for security issues.
```

## Conventions for Claude Code

Claude Code should:

- keep Domain independent
- keep business logic out of controllers and Infrastructure
- prefer constructor injection
- use EF Core migrations for schema work
- use strongly typed options
- keep generated code minimal and explicit
- add tests for both happy paths and failure paths
- verify changes before finishing

These same conventions are also surfaced through `AGENTS.md`, `.github/copilot-instructions.md`, and `prompts/` for cross-tool usage.

## EF Core Guidance

- SQL Server is the default provider
- keep mappings in configuration classes
- use concurrency tokens for aggregates that are edited concurrently
- bound pagination inputs
- use explicit transactions for multi-step write workflows
- avoid introducing generic repositories by default

For tests, prefer:

- SQLite for lightweight integration coverage
- Testcontainers when provider behavior or external dependencies matter

## Azure Guidance

This template assumes Azure support is important, but keeps vendor-specific code out of the Domain and Application layers.

Recommended defaults:

- Managed Identity and Entra ID for authentication
- Azure Key Vault for secrets
- Azure App Configuration for centralized configuration
- Azure Storage for blob/file workloads
- Azure Service Bus for asynchronous messaging

Keep Azure SDK clients behind Infrastructure implementations or boundary interfaces where business logic depends on them.

## Why This Repo Exists

This is a template repository, not a finished product. Its job is to give human contributors and multiple AI coding tools a clean, opinionated starting point for building maintainable enterprise .NET services quickly.
