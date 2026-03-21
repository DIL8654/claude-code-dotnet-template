# API Contract Review

Human-facing guide for the `api-contract-review` skill.

## Purpose

Use this skill to review ASP.NET Core Web API contracts for correctness, consistency, and client safety.

## When To Use It

- Before releasing endpoint changes
- When reviewing controller or endpoint PRs
- When validating route design, versioning, or status codes
- When checking backward compatibility for public APIs

## Quick Reference

| Focus Area | What Good Looks Like |
|------------|----------------------|
| HTTP semantics | Correct verbs and status codes |
| Versioning | Public APIs are versioned clearly |
| Contracts | DTOs over the wire, not EF entities |
| Errors | `ProblemDetails` or `ValidationProblemDetails` |
| Collections | Pagination is bounded |

## Review Checklist

- Routes are resource-oriented and versioned
- Controllers stay thin
- Error responses are consistent
- No `200 OK` responses for failure payloads
- Breaking contract changes are intentional and documented

## Typical Prompt

```text
Use the api-contract-review skill on these controller changes.
```
