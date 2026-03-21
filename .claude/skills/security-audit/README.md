# Security Audit

Human-facing guide for the `security-audit` skill.

## Purpose

Use this skill to review a .NET service for secure defaults in input handling, authentication, authorization, secret management, and operational exposure.

## When To Use It

- Before releasing an API or service
- When adding authentication or authorization
- When reviewing configuration and secret handling
- During dependency and logging review

## Quick Reference

| Area | What To Check |
|------|---------------|
| Input | Validation, limits, and trusted boundaries |
| Access | AuthN and policy enforcement |
| Secrets | No hardcoded or leaked credentials |
| Errors | Safe client responses, internal details logged only |
| Operations | Environment settings and risky dependencies |

## Review Checklist

- External input is validated
- Sensitive endpoints are protected correctly
- Secrets are externalized
- Logs and errors do not leak internal details
- Production settings differ safely from development

## Typical Prompt

```text
Use the security-audit skill to review this API and its configuration.
```
