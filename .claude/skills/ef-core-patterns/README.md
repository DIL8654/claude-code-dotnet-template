# EF Core Patterns

Human-facing guide for the `ef-core-patterns` skill.

## Purpose

Use this skill when working with EF Core models, queries, migrations, concurrency, and transaction behavior in the Infrastructure layer.

## When To Use It

- Adding or changing entities
- Reviewing DbContext boundaries
- Writing query logic
- Creating migrations
- Designing persistence tests

## Quick Reference

| Area | Preferred Pattern |
|------|-------------------|
| Mapping | `IEntityTypeConfiguration<T>` classes |
| Schema changes | EF Core migrations |
| Reads | `AsNoTracking()` where appropriate |
| Updates | Explicit concurrency handling when needed |
| Pagination | Bounded `Skip` / `Take` |

## Review Checklist

- Mapping is explicit
- Domain is not polluted with persistence concerns
- Queries are bounded
- Transactions are clear
- Repositories exist only when they add value

## Typical Prompt

```text
Use the ef-core-patterns skill to review this DbContext and repository code.
```
