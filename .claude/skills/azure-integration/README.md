# Azure Integration

Human-facing guide for the `azure-integration` skill.

## Purpose

Use this skill when adding Azure services to a .NET application while keeping cloud concerns isolated, typed, and production-safe.

## When To Use It

- Adding Key Vault
- Adding App Configuration
- Wiring Blob Storage
- Integrating Service Bus
- Replacing secret-based auth with Managed Identity

## Quick Reference

| Area | Preferred Pattern |
|------|-------------------|
| Authentication | Managed Identity / Entra ID |
| Configuration | Strongly typed options |
| Placement | Azure SDKs in Infrastructure only |
| Secrets | Secure stores, never hardcoded |
| Local dev | Explicit fallback path |

## Review Checklist

- No secrets are committed or hardcoded
- Azure SDK types stay out of Domain and Application
- Required settings are validated
- Cloud failures are logged meaningfully
- Messaging and storage boundaries are explicit

## Typical Prompt

```text
Use the azure-integration skill to wire Azure Key Vault and App Configuration.
```
