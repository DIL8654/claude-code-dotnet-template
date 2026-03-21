---
name: logging-patterns
description: Keep logs structured, correlated, low-noise, and safe for production support. Use when adding operational logging, reviewing observability, or improving diagnostics in .NET services.
---

# Logging Patterns Skill

Write logs that help operators diagnose real problems without flooding the system or leaking sensitive data.

## When to Use
- Adding logs around business or infrastructure flows
- Reviewing observability before release
- Introducing request or message correlation
- Cleaning up noisy or inconsistent logs

---

## Quick Reference: Common Issues

| Issue | Symptom | Impact |
|-------|---------|--------|
| String interpolation logging | `$"User {id} failed"` everywhere | Loses structured properties |
| Over-logging | Info logs on every small branch | High noise, low signal |
| Sensitive data leakage | Tokens, secrets, or PII in logs | Security and compliance risk |
| Missing context | Failures logged without identifiers | Harder troubleshooting |
| Inconsistent event style | Same event logged in many shapes | Harder searching and dashboards |

---

## Structured Logging

```csharp
// BAD: Interpolated string
logger.LogInformation($"Processing order {orderId}");

// GOOD: Structured property
logger.LogInformation("Processing order {OrderId}", orderId);
```

Prefer named properties that operators and tools can query reliably.

---

## What to Log

Good candidates:
- Start and completion of important operations
- Failures with enough context to diagnose them
- State transitions
- External dependency failures
- Authentication and authorization failures where appropriate

Avoid:
- Logging every loop iteration
- Repeating the same event at multiple layers
- Dumping entire payloads by default

---

## Correlation

Include request IDs, trace IDs, message IDs, tenant IDs, or aggregate IDs when they help connect related events.

```csharp
logger.LogWarning(
    "Service Bus message handling failed for {MessageId} on {Subscription}",
    messageId,
    subscriptionName);
```

---

## Sensitive Data Rules

Never log:
- Secrets
- Access tokens
- Connection strings
- Raw passwords
- Full personal data payloads unless there is a clear approved reason

If a value is sensitive, log an identifier or redacted version instead.

---

## Log Levels

| Level | Use For |
|------|---------|
| Trace | Deep diagnostics, usually off in production |
| Debug | Developer investigation |
| Information | Important business or system milestones |
| Warning | Unexpected but recoverable conditions |
| Error | Failed operations requiring attention |
| Critical | Severe failures affecting service health |

---

## Logging Checklist

### 1. Structure
- [ ] Named properties are used
- [ ] Events are consistent across similar flows
- [ ] Important identifiers are included

### 2. Signal Quality
- [ ] Logs are not overly chatty
- [ ] Info-level logs reflect meaningful events
- [ ] Duplicate logs across layers are minimized

### 3. Safety
- [ ] No secrets or sensitive payloads are logged
- [ ] Exception messages are reviewed for leakage
- [ ] Production logging defaults are appropriate

### 4. Operations
- [ ] Failures include enough context to investigate
- [ ] Correlation information exists where needed
- [ ] Logging supports health, incident, and support workflows

---

## Token Optimization

For log review:
1. Search logger usage patterns first.
2. Inspect middleware, controllers, and external adapters.
3. Focus on error and warning logs before info-level cleanup.

Suggested commands:

```bash
rg "LogTrace|LogDebug|LogInformation|LogWarning|LogError|LogCritical" src
rg "\\$\"" src
rg "token|secret|password|connectionstring" src -i
```
