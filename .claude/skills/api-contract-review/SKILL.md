---
name: api-contract-review
description: Review ASP.NET Core Web API contracts for HTTP semantics, versioning, backward compatibility, and response consistency. Use when the user asks to review an API, check endpoints, validate REST behavior, or before releasing contract changes.
---

# API Contract Review Skill

Audit ASP.NET Core Web API design for correctness, consistency, and compatibility.

## When to Use
- User asks "review this API" or "check these endpoints"
- Before releasing API changes
- Reviewing pull requests with controller, endpoint, or contract changes
- Checking backward compatibility for public APIs

---

## Quick Reference: Common Issues

| Issue | Symptom | Impact |
|-------|---------|--------|
| Wrong HTTP verb | `POST` used for idempotent retrieval or update | Confusing semantics, caching issues, harder clients |
| Missing versioning | `/api/users` instead of `/api/v1/users` | Breaking changes affect all consumers |
| Entity leak | EF Core entity returned directly from controller | Exposes internals, persistence shape, concurrency fields |
| `200 OK` with error payload | Error body returned with HTTP 200 | Breaks client error handling |
| Inconsistent route naming | `/getUsers` vs `/users` | Makes APIs harder to learn and maintain |

---

## HTTP Verb Semantics

### Verb Selection Guide

| Verb | Use For | Idempotent | Safe | Request Body |
|------|---------|------------|------|--------------|
| GET | Retrieve resource | Yes | Yes | No |
| POST | Create resource or invoke non-idempotent action | No | No | Yes |
| PUT | Replace resource | Yes | No | Yes |
| PATCH | Partial update | Usually | No | Yes |
| DELETE | Remove resource | Yes | No | Optional |

### Common Mistakes

```csharp
// BAD: POST for retrieval
[HttpPost("search")]
public async Task<ActionResult<IReadOnlyList<UserResponse>>> SearchAsync(
    [FromBody] UserSearchRequest request,
    CancellationToken cancellationToken)
{
    return Ok(await service.SearchAsync(request, cancellationToken));
}

// GOOD: GET with query parameters when the query is simple
[HttpGet]
public async Task<ActionResult<IReadOnlyList<UserResponse>>> SearchAsync(
    [FromQuery] string name,
    [FromQuery] string? email,
    CancellationToken cancellationToken)
{
    return Ok(await service.SearchAsync(name, email, cancellationToken));
}

// BAD: GET with side effects
[HttpGet("{id:guid}/activate")]
public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken cancellationToken)
{
    await service.ActivateAsync(id, cancellationToken);
    return NoContent();
}

// GOOD: POST or PATCH for state change
[HttpPost("{id:guid}/activate")]
public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken cancellationToken)
{
    await service.ActivateAsync(id, cancellationToken);
    return NoContent();
}

// BAD: POST for idempotent replacement
[HttpPost("{id:guid}")]
public async Task<ActionResult<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
{
    return Ok(await service.ReplaceAsync(id, request, cancellationToken));
}

// GOOD: PUT for replacement, PATCH for partial update
[HttpPut("{id:guid}")]
public async Task<ActionResult<UserResponse>> ReplaceAsync(Guid id, ReplaceUserRequest request, CancellationToken cancellationToken)
{
    return Ok(await service.ReplaceAsync(id, request, cancellationToken));
}

[HttpPatch("{id:guid}")]
public async Task<ActionResult<UserResponse>> PatchAsync(Guid id, PatchUserRequest request, CancellationToken cancellationToken)
{
    return Ok(await service.PatchAsync(id, request, cancellationToken));
}
```

---

## API Versioning

### Strategies

| Strategy | Example | Pros | Cons |
|----------|---------|------|------|
| URL path | `/api/v1/users` | Clear and visible | URL changes between versions |
| Header | `Accept: application/vnd.company.v1+json` | Clean URLs | Harder to discover and test |
| Query string | `/api/users?api-version=1` | Easy to introduce | Easy to forget or misuse |

### Recommended: URL Path

```csharp
// GOOD: Explicit versioned routes
[ApiController]
[Route("api/v1/users")]
public sealed class UsersController : ControllerBase
{
}

[ApiController]
[Route("api/v2/users")]
public sealed class UsersV2Controller : ControllerBase
{
}

// BAD: Unversioned public API
[ApiController]
[Route("api/users")]
public sealed class LegacyUsersController : ControllerBase
{
}
```

### Version Checklist
- [ ] All public APIs are versioned
- [ ] Internal APIs are clearly identified if unversioned
- [ ] Breaking changes are introduced through a new version
- [ ] Deprecated versions have a removal plan

---

## Request and Response Design

### DTO vs Entity

```csharp
// BAD: EF Core entity returned directly
[HttpGet("{id:guid}")]
public async Task<ActionResult<User>> GetAsync(Guid id, CancellationToken cancellationToken)
{
    User? user = await dbContext.Users.FindAsync([id], cancellationToken);
    return user is null ? NotFound() : Ok(user);
}

// GOOD: Response DTO returned from application or mapping layer
[HttpGet("{id:guid}")]
public async Task<ActionResult<UserResponse>> GetAsync(Guid id, CancellationToken cancellationToken)
{
    UserResponse user = await service.GetAsync(id, cancellationToken);
    return Ok(user);
}
```

### Response Consistency

```csharp
// BAD: Inconsistent response shapes
[HttpGet]
public Task<IReadOnlyList<UserResponse>> ListAsync(CancellationToken cancellationToken) { }

[HttpGet("{id:guid}")]
public Task<UserResponse> GetAsync(Guid id, CancellationToken cancellationToken) { }

[HttpGet("count")]
public Task<int> CountAsync(CancellationToken cancellationToken) { }

// GOOD: Stable, explicit shapes
[HttpGet]
public async Task<ActionResult<PagedResponse<UserResponse>>> ListAsync(
    [FromQuery] int skip = 0,
    [FromQuery] int take = 20,
    CancellationToken cancellationToken = default)
{
    return Ok(await service.ListAsync(skip, take, cancellationToken));
}

[HttpGet("count")]
public async Task<ActionResult<object>> CountAsync(CancellationToken cancellationToken)
{
    int count = await service.CountAsync(cancellationToken);
    return Ok(new { Count = count });
}
```

### Pagination

```csharp
// BAD: Unbounded collection endpoint
[HttpGet]
public async Task<ActionResult<IReadOnlyList<UserResponse>>> ListAsync(CancellationToken cancellationToken)
{
    return Ok(await service.ListAllAsync(cancellationToken));
}

// GOOD: Bounded pagination
[HttpGet]
public async Task<ActionResult<PagedResponse<UserResponse>>> ListAsync(
    [FromQuery] int skip = 0,
    [FromQuery] int take = 20,
    CancellationToken cancellationToken = default)
{
    return Ok(await service.ListAsync(skip, take, cancellationToken));
}
```

---

## HTTP Status Codes

### Success Codes

| Code | When to Use | Response Body |
|------|-------------|---------------|
| 200 OK | Successful GET, PUT, PATCH | Resource or operation result |
| 201 Created | Successful POST that created a resource | Created resource plus `Location` header |
| 202 Accepted | Async command accepted for later processing | Optional status payload |
| 204 No Content | Successful DELETE or command with no body | Empty |

### Error Codes

| Code | When to Use | Common Mistake |
|------|-------------|----------------|
| 400 Bad Request | Invalid format, invalid arguments, validation failure | Used for not found |
| 401 Unauthorized | Authentication required or failed | Confused with 403 |
| 403 Forbidden | Authenticated but not allowed | Returned as 401 |
| 404 Not Found | Resource does not exist | Returned as 400 |
| 409 Conflict | Duplicate resource or concurrency conflict | Returned as 400 |
| 422 Unprocessable Entity | Valid syntax, invalid business meaning | Flattened into 400 |
| 500 Internal Server Error | Unexpected server failure | Internal details exposed |

### Anti-Pattern: `200 OK` with Error Body

```csharp
// BAD: Error encoded as success
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
{
    UserResponse? user = await service.TryGetAsync(id, cancellationToken);

    if (user is null)
    {
        return Ok(new { Error = "User not found" });
    }

    return Ok(user);
}

// GOOD: Correct status code
[HttpGet("{id:guid}")]
public async Task<ActionResult<UserResponse>> GetAsync(Guid id, CancellationToken cancellationToken)
{
    UserResponse user = await service.GetAsync(id, cancellationToken);
    return Ok(user);
}
```

---

## Error Response Format

### Consistent Error Structure

Prefer `ProblemDetails` or `ValidationProblemDetails` rather than inventing a new ad hoc shape for every controller.

```csharp
// GOOD: Centralized ProblemDetails-based handling
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception");

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred."
            }
        });
    }
}
```

### Security: Do Not Expose Internals

```csharp
// BAD: Stack trace or exception message exposed directly
app.MapGet("/boom", () =>
{
    throw new InvalidOperationException("Database password was invalid.");
});

// GOOD: Log details server-side, return safe error details to clients
logger.LogError(exception, "Unexpected failure while handling {Path}", httpContext.Request.Path);
```

---

## Backward Compatibility

### Breaking Changes

| Change | Breaking? | Safer Migration |
|--------|-----------|-----------------|
| Remove endpoint | Yes | Deprecate first, remove in next version |
| Remove field from response | Yes | Keep field until next version boundary |
| Add required request field | Yes | Make it optional first |
| Change property type | Yes | Introduce a new field and deprecate the old |
| Rename route or parameter | Yes | Support both temporarily |
| Change status code contract | Often | Document and version it |

### Usually Non-Breaking Changes
- Add optional request fields
- Add new response fields when clients tolerate unknown fields
- Add new endpoints
- Add new optional query parameters

### Deprecation Pattern

```csharp
[ApiController]
[Route("api/v1/users")]
public sealed class UsersController : ControllerBase
{
    [Obsolete("Use GET /api/v1/users?email={email} instead.")]
    [HttpGet("by-email")]
    public Task<ActionResult<UserResponse>> GetByEmailLegacyAsync([FromQuery] string email, CancellationToken cancellationToken) =>
        GetByEmailAsync(email, cancellationToken);

    [HttpGet]
    public async Task<ActionResult<UserResponse>> GetByEmailAsync([FromQuery] string email, CancellationToken cancellationToken)
    {
        return Ok(await service.GetByEmailAsync(email, cancellationToken));
    }
}
```

---

## API Review Checklist

### 1. HTTP Semantics
- [ ] GET is retrieval-only
- [ ] POST is used for creation or non-idempotent commands
- [ ] PUT is used for full replacement
- [ ] PATCH is used for partial change
- [ ] DELETE is idempotent

### 2. URL Design
- [ ] Routes are versioned
- [ ] Routes use nouns, not verbs
- [ ] Collection routes are plural
- [ ] Child relationships are hierarchical where useful
- [ ] Naming is consistent across the API

### 3. Request Handling
- [ ] Request DTOs are used
- [ ] Validation is explicit
- [ ] Size and pagination limits are bounded
- [ ] Cancellation tokens flow to async operations

### 4. Response Design
- [ ] Response DTOs are used
- [ ] Response shapes are consistent
- [ ] Collection endpoints are paginated where needed
- [ ] Success and failure status codes are correct

### 5. Error Handling
- [ ] `ProblemDetails` or `ValidationProblemDetails` is used consistently
- [ ] Internal details are not exposed
- [ ] 4xx vs 5xx distinction is correct
- [ ] Concurrency or duplicate scenarios map to `409` when appropriate

### 6. Compatibility
- [ ] No accidental breaking changes in the current version
- [ ] Deprecated endpoints are documented
- [ ] Migration path exists for changes that affect clients

---

## Token Optimization

For large APIs:
1. List controllers or endpoint groups first.
2. Review 2-3 representative endpoints for patterns before reading everything.
3. Check exception handling and `ProblemDetails` configuration once.
4. Search for anti-patterns such as direct EF entity responses, unversioned routes, or `Ok(...)` error payloads.

Suggested commands:

```bash
rg "ControllerBase|MapGet|MapPost|MapPut|MapPatch|MapDelete" src
rg "return Ok\\(new \\{.*error|return Ok\\(.*Problem" src
rg "Route\\(\"api/(?!v[0-9])|MapGroup\\(\"/api/(?!v[0-9])" src -P
rg "ActionResult<.*Entity>|Task<.*Entity>" src
```
