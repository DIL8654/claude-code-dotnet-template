# Testing Patterns

Human-facing guide for the `testing-patterns` skill.

## Purpose

Use this skill to add or improve .NET tests using xUnit, FluentAssertions, NSubstitute, and focused integration testing patterns.

## When To Use It

- Adding unit tests for services or domain logic
- Adding API integration tests
- Testing EF Core persistence behavior
- Improving negative-path coverage

## Quick Reference

| Test Level | Best Use |
|------------|----------|
| Unit | Business logic and orchestration |
| Integration | HTTP, middleware, serialization, persistence |
| Persistence-focused | EF Core mappings, migrations, query behavior |

## Review Checklist

- Positive and negative paths are covered
- Test names describe behavior clearly
- Setup is deterministic
- Mocks are limited to real boundaries
- HTTP and persistence behavior are tested at the correct level

## Typical Prompt

```text
Use the testing-patterns skill to add tests for this feature.
```
