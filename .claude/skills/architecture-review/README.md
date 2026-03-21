# Architecture Review

Human-facing guide for the `architecture-review` skill.

## Purpose

Use this skill to review Clean Architecture boundaries, dependency direction, and whether code is living in the correct layer.

## When To Use It

- Before merging structural changes
- When adding projects or project references
- When investigating architectural drift
- During technical design review

## Quick Reference

| Layer | Owns | Must Avoid |
|------|------|------------|
| Domain | Business rules and core model | EF Core, HTTP, Azure SDKs |
| Application | Use cases, DTOs, interfaces | Vendor or persistence details |
| Infrastructure | EF Core, SDKs, implementations | Core business rules |
| WebApi | Controllers, middleware, composition root | Domain logic, persistence logic |

## Review Checklist

- Project references point inward only
- Domain is framework-free
- Application owns orchestration and boundaries
- Infrastructure owns implementation details
- WebApi stays thin

## Typical Prompt

```text
Use the architecture-review skill to review this solution structure.
```
