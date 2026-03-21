# Portable Prompts

This folder provides cross-tool prompt entrypoints for assistants that do not automatically understand the `.claude/` folder structure.

## Why This Exists

Different tools load repository guidance differently:

- Claude Code can use `CLAUDE.md` and `.claude/`
- Codex-style agents can use `AGENTS.md`
- GitHub Copilot can use `.github/copilot-instructions.md`
- Local LLM tools often need explicit prompt files or pasted instructions

The files in this folder give you stable, portable prompt starting points for those local or generic tools.

## Available Prompt Entry Points

| File | Use For |
|------|---------|
| `system-dotnet-template.md` | General-purpose repository instructions |
| `use-an-agent.md` | How to invoke one of the specialized agent prompts |
| `use-a-skill.md` | How to apply one of the skill playbooks |

## Typical Local LLM Workflow

1. Load `AGENTS.md` or `prompts/system-dotnet-template.md`.
2. If the task is specialized, also load one file from `.claude/agents/`.
3. If the task needs a checklist or rubric, also load one file from `.claude/skills/.../SKILL.md`.
4. Ask the model to follow those files while working on the relevant code.

## Example

```text
Read AGENTS.md, prompts/use-an-agent.md, .claude/agents/dotnet-engineer.md, and .claude/skills/testing-patterns/SKILL.md.
Use them while implementing this API feature and its tests.
```
