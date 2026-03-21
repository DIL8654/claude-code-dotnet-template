---
name: azure-integration
description: Add Azure services to .NET applications with Managed Identity, typed options, infrastructure isolation, and production-safe configuration. Use when wiring Key Vault, App Configuration, Storage, Service Bus, or Azure Identity patterns.
---

# Azure Integration Skill

Integrate Azure services into a .NET application without leaking cloud-specific concerns into Domain or Application layers.

## When to Use
- Adding Azure Key Vault or App Configuration
- Wiring Blob Storage or Queue/Service Bus clients
- Replacing connection-string auth with Managed Identity
- Reviewing Azure SDK usage in an existing codebase

---

## Quick Reference: Common Issues

| Issue | Symptom | Impact |
|-------|---------|--------|
| Secret in code | Connection string or key committed in source | Immediate credential exposure |
| SDK leakage | `BlobServiceClient` used in Application code | Vendor coupling spreads upward |
| Untyped configuration | Direct `configuration["Azure:..."]` reads everywhere | Fragile and inconsistent config handling |
| Identity mismatch | Local dev uses secrets, production uses another auth path | Environment drift and hard-to-debug failures |
| Missing resilience | No retries or timeout boundaries around cloud calls | Operational instability |

---

## Design Rules

- Keep Azure SDK clients in Infrastructure
- Prefer Managed Identity and Entra ID auth
- Bind settings into typed options
- Expose application-facing abstractions when business logic depends on cloud behavior
- Keep local development fallback explicit, not accidental

---

## Configuration Patterns

### Typed Options

```csharp
public sealed class AzureStorageOptions
{
    public const string SectionName = "Azure:Storage";

    public string? BlobServiceUri { get; init; }
}

services.AddOptions<AzureStorageOptions>()
    .Bind(configuration.GetSection(AzureStorageOptions.SectionName))
    .Validate(options => !string.IsNullOrWhiteSpace(options.BlobServiceUri), "Azure Storage endpoint is required.");
```

### Avoid Ad Hoc Configuration Reads

```csharp
// BAD: Raw string lookup spread through the codebase
string endpoint = configuration["Azure:Storage:BlobServiceUri"]!;

// GOOD: Centralized, typed configuration
AzureStorageOptions options = serviceProvider
    .GetRequiredService<IOptions<AzureStorageOptions>>()
    .Value;
```

---

## Authentication Patterns

### Preferred Default

```csharp
// GOOD: Managed Identity / DefaultAzureCredential
TokenCredential credential = new DefaultAzureCredential();
BlobServiceClient client = new(new Uri(options.BlobServiceUri!), credential);
```

### Avoid Secret-Based Auth by Default

```csharp
// BAD: Secret-based auth hardcoded in code
BlobServiceClient client = new(connectionStringFromSourceControl);
```

Use secret-based local fallback only when necessary, and only through secure configuration sources.

---

## Service-Specific Guidance

### Key Vault
- Use for secrets only
- Keep secret names stable and environment-aware
- Do not read secrets directly in Domain or Application

### App Configuration
- Use for centralized app settings and feature toggles
- Keep option binding in composition root or Infrastructure setup

### Blob Storage
- Hide SDK calls behind a storage abstraction when used by business workflows
- Normalize naming, containers, and content-type behavior

### Service Bus
- Keep message contracts explicit
- Separate send, receive, and handler concerns
- Make retry, poison-message, and idempotency expectations visible

---

## Infrastructure Boundary Examples

```csharp
// GOOD: Application-facing boundary
public interface IFileStorage
{
    Task<Uri> UploadAsync(Stream content, string fileName, CancellationToken cancellationToken);
}

// GOOD: Azure implementation in Infrastructure
public sealed class AzureBlobFileStorage(BlobServiceClient blobServiceClient) : IFileStorage
{
    public async Task<Uri> UploadAsync(Stream content, string fileName, CancellationToken cancellationToken)
    {
        BlobContainerClient container = blobServiceClient.GetBlobContainerClient("uploads");
        BlobClient blob = container.GetBlobClient(fileName);
        await blob.UploadAsync(content, overwrite: true, cancellationToken);
        return blob.Uri;
    }
}
```

---

## Azure Integration Checklist

### 1. Security
- [ ] No secrets are hardcoded
- [ ] Managed Identity or Entra ID is the default auth path
- [ ] Secret fallback is configuration-driven only

### 2. Layering
- [ ] Azure SDK types stay in Infrastructure
- [ ] Domain and Application depend on abstractions only when needed
- [ ] WebApi only wires configuration and registrations

### 3. Configuration
- [ ] Options are strongly typed
- [ ] Required settings are validated
- [ ] Environment-specific behavior is explicit

### 4. Operations
- [ ] Retry and timeout behavior is considered
- [ ] Failure modes are logged meaningfully
- [ ] Idempotency is considered for messaging flows

---

## Token Optimization

For existing solutions:
1. Search for Azure SDK types and configuration string lookups.
2. Inspect DI registration and options classes.
3. Review only the relevant Azure service implementation.

Suggested commands:

```bash
rg "Azure\\.|DefaultAzureCredential|BlobServiceClient|ServiceBusClient|SecretClient" src
rg 'configuration\\["Azure:' src
rg "IOptions<|AddOptions<|Bind\\(" src
```
