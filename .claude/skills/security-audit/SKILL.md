---
name: security-audit
description: Review .NET services for safe defaults in input handling, authentication, authorization, secret management, logging exposure, and configuration hygiene. Use before release, during reviews, or when hardening an API or integration.
---

# Security Audit Skill

Review a .NET service for practical security weaknesses and unsafe defaults.

## When to Use
- Before releasing an API or background service
- When adding authentication or authorization
- When reviewing secrets, configuration, or Azure integrations
- During dependency and logging review

---

## Quick Reference: Common Issues

| Issue | Symptom | Impact |
|-------|---------|--------|
| Missing validation | Raw external input trusted too early | Injection, malformed state, bad data |
| Weak authorization | Endpoint protected by auth but not policy | Over-broad access |
| Secret leakage | Keys or tokens in config files or logs | Credential exposure |
| Verbose production errors | Stack traces or internal messages returned | Attack surface disclosure |
| Unsafe defaults | Debug settings or permissive CORS in production | Avoidable exposure |

---

## Input Handling

- Validate request models
- Bound sizes, counts, and pagination
- Treat all external input as untrusted
- Normalize input only when the business meaning is clear

```csharp
// GOOD: Explicit constraints
public sealed record CreateUserRequest(
    [property: Required, MaxLength(200)] string Name,
    [property: EmailAddress] string Email);
```

---

## Authentication and Authorization

### Review Questions
- Is authentication configured correctly?
- Are endpoints protected by the correct policies?
- Is authorization checked at the right boundary?
- Are service-to-service identities scoped minimally?

Prefer policy-based authorization to ad hoc role checks scattered through business logic.

---

## Secrets and Configuration

- Never hardcode secrets
- Keep secrets in Key Vault or another secure store
- Avoid committing production-like values into appsettings files
- Review environment variables and CI/CD injection paths

```csharp
// BAD
string apiKey = "super-secret-key";

// GOOD
string apiKey = configuration["ExternalApis:Payments:ApiKey"]
    ?? throw new InvalidOperationException("Payments API key is required.");
```

---

## Logging and Error Exposure

- Review logs for secrets, PII, and token leakage
- Use safe `ProblemDetails`-style responses
- Keep internal exception details server-side

```csharp
// BAD
return Results.Problem(detail: exception.ToString());

// GOOD
logger.LogError(exception, "Unexpected failure");
return Results.Problem(title: "An unexpected error occurred.");
```

---

## Dependency and Runtime Hygiene

- Review package versions and known risky dependencies
- Prefer current supported framework versions
- Remove unused packages that enlarge attack surface
- Check dev-only settings do not bleed into production

---

## Security Audit Checklist

### 1. Input
- [ ] Request validation exists
- [ ] Payload sizes and pagination are bounded
- [ ] Unsafe deserialization or parsing patterns are absent

### 2. Access Control
- [ ] Authentication is configured correctly
- [ ] Authorization is policy-based where appropriate
- [ ] Sensitive operations are protected explicitly

### 3. Secrets
- [ ] No secrets are hardcoded
- [ ] Secret stores are used for production
- [ ] Logging and telemetry do not leak credentials

### 4. Error Handling
- [ ] Internal details are not returned to clients
- [ ] Problem responses are safe and consistent
- [ ] Production and development behavior are separated

### 5. Operations
- [ ] CORS, headers, and environment settings are reviewed
- [ ] Package risk is reviewed
- [ ] Security-sensitive paths have negative tests

---

## Token Optimization

For a security pass:
1. Review auth configuration and middleware first.
2. Search for secrets, permissive settings, and verbose errors.
3. Sample high-risk endpoints and external integrations.

Suggested commands:

```bash
rg "Authorize|AllowAnonymous|AddAuthentication|AddAuthorization" src
rg "password|secret|token|apikey|connectionstring" src -i
rg "Problem\\(|Exception\\.ToString\\(|ex\\.Message" src
rg "Cors|AllowAnyOrigin|AllowAnyHeader|AllowAnyMethod" src
```
