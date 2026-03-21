# How to Use an Agent Prompt

This repository stores specialized agent prompts in `.claude/agents/`.

Use one when the task has a clear specialization, for example:

- `dotnet-engineer` for normal ASP.NET Core implementation work
- `csharp-architect` for structure and boundary decisions
- `ef-core-engineer` for persistence and migrations
- `azure-engineer` for Azure integrations
- `security-engineer` for hardening and auth
- `test-automator` for testing work
- `code-reviewer` for review tasks
- `devops-engineer` for CI/CD and deployment automation

## Example Prompt

```text
Read AGENTS.md and .claude/agents/ef-core-engineer.md.
Follow those instructions while reviewing and updating the persistence layer.
```
