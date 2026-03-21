# Skills

Skills are reusable prompts that teach Claude Code how this repository expects modern .NET work to be done.

## Available Skills

| Skill | Purpose |
|------|---------|
| [clean-code](clean-code/README.md) | Keep C# code small, explicit, readable, and easy to refactor |
| [architecture-review](architecture-review/README.md) | Audit boundaries, project references, and dependency flow |
| [design-patterns](design-patterns/README.md) | Apply patterns only when they simplify the design |
| [api-contract-review](api-contract-review/README.md) | Review REST semantics, versioning, and response consistency |
| [ef-core-patterns](ef-core-patterns/README.md) | Guide EF Core mappings, queries, migrations, and transaction choices |
| [azure-integration](azure-integration/README.md) | Add Azure services with Managed Identity and typed configuration |
| [logging-patterns](logging-patterns/README.md) | Keep logs structured, correlated, and safe |
| [testing-patterns](testing-patterns/README.md) | Produce readable xUnit coverage for positive and negative scenarios |
| [security-audit](security-audit/README.md) | Review input validation, auth, secrets, and safe defaults |

## Structure Convention

Each skill folder contains:

| File | Purpose | Audience |
|------|---------|----------|
| `SKILL.md` | Detailed instructions and review workflow | Claude Code |
| `README.md` | Human-readable summary, use cases, and quick reference | Engineers |

## Conventions

- Keep skills focused and reusable.
- Prefer .NET examples, not generic pseudocode.
- Optimize for production-ready defaults, not tutorial prose.

## How to Use

- Read the skill README when you want a quick summary of what the skill is for.
- Read the `SKILL.md` when you want the full audit/template guidance and examples.
- Prefer invoking the skill by name when asking Claude Code for reviews or implementation help.
