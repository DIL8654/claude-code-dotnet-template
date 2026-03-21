# Logging Patterns

Human-facing guide for the `logging-patterns` skill.

## Purpose

Use this skill to keep application logs structured, useful in production, and safe from sensitive data leakage.

## When To Use It

- Adding new operational logs
- Reviewing noisy or inconsistent logging
- Introducing correlation identifiers
- Hardening observability before deployment

## Quick Reference

| Concern | Good Default |
|---------|--------------|
| Format | Structured logs with named properties |
| Correlation | Include request, message, or aggregate IDs |
| Noise | Log meaningful milestones, not every branch |
| Safety | Never log secrets or sensitive payloads |

## Review Checklist

- Structured properties are used consistently
- Info logs are meaningful
- Error logs include actionable context
- No secrets, tokens, or sensitive payloads appear in logs
- Correlation is present where useful

## Typical Prompt

```text
Use the logging-patterns skill to review these logs and middleware changes.
```
