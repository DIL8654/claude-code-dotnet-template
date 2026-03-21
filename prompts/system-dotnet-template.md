# Portable System Prompt for This Repository

Use this repository as a starter kit for building modern ASP.NET Core services with Clean Architecture, EF Core, Azure-ready boundaries, and strong testing defaults.

Follow these core rules:

- Preserve inward-only dependencies.
- Keep Domain independent.
- Keep Application focused on use cases and abstractions.
- Keep Infrastructure focused on EF Core and external services.
- Keep WebApi focused on HTTP and composition root concerns.
- Prefer explicit, readable C#.
- Use strongly typed configuration.
- Use EF Core migrations for schema changes.
- Add tests for happy and failure paths.
- Verify changes before finishing.

If the task is specialized, also load the relevant files from:

- `.claude/agents/`
- `.claude/skills/`
- `AGENTS.md`
- `CLAUDE.md`
