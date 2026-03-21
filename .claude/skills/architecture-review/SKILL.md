---
name: architecture-review
description: Review Clean Architecture boundaries, dependency direction, layering decisions, and abstraction quality in .NET solutions. Use when evaluating project structure, new dependencies, service boundaries, or architectural drift.
---

# Architecture Review Skill

Audit a .NET solution for boundary integrity, dependency direction, and long-term maintainability.

## When to Use
- Reviewing a new solution or template
- Evaluating pull requests that add new projects or references
- Checking whether business logic is in the wrong layer
- Investigating architectural drift in an existing codebase

---

## Quick Reference: Common Issues

| Issue | Symptom | Impact |
|-------|---------|--------|
| Inward dependency violation | Domain references Infrastructure or WebApi | Business logic becomes framework-coupled |
| Thin abstraction with no value | Interface with one trivial implementation and no boundary role | Extra indirection with no payoff |
| Fat controller | Controllers performing orchestration, validation, and persistence | Harder testing, duplicated behavior |
| Infrastructure leakage | Azure SDK types or EF Core entities appear in Application | Vendor and persistence coupling |
| Anemic boundary | Application layer just forwards calls without use-case logic | Layering exists on paper only |

---

## Target Layer Responsibilities

| Layer | Owns | Must Avoid |
|------|------|------------|
| Domain | Entities, value objects, domain rules | EF Core, HTTP, Azure SDKs, controllers |
| Application | Use cases, DTOs, interfaces, orchestration | Database-specific code, transport models |
| Infrastructure | EF Core, repositories, SDK clients, file/storage integrations | Core business rules |
| WebApi | Controllers, middleware, auth wiring, composition root | Domain logic, persistence logic |

### Dependency Rule

Dependencies should point inward:

`WebApi -> Infrastructure -> Application -> Domain`

`WebApi -> Application` is also fine when Infrastructure is only wired at composition root.

---

## Boundary Review

### Project Reference Anti-Patterns

```xml
<!-- BAD: Domain depends on Infrastructure -->
<ItemGroup>
  <ProjectReference Include="..\Infrastructure\Infrastructure.csproj" />
</ItemGroup>

<!-- GOOD: Domain stays isolated -->
<Project Sdk="Microsoft.NET.Sdk">
</Project>
```

### Application Boundary

```csharp
// BAD: Application coupled to EF Core directly
public sealed class CreateOrderHandler(AppDbContext dbContext)
{
    public async Task<Guid> HandleAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var entity = new Order { CustomerName = request.CustomerName };
        dbContext.Orders.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

// GOOD: Application depends on abstraction
public sealed class CreateOrderHandler(IOrderRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<Guid> HandleAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        Order order = Order.Create(request.CustomerName);
        await repository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return order.Id;
    }
}
```

### Web Boundary

```csharp
// BAD: Controller contains business logic
[HttpPost]
public async Task<IActionResult> CreateAsync(CreateOrderHttpRequest request, CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(request.CustomerName))
    {
        return BadRequest();
    }

    Order order = new(request.CustomerName.Trim());
    dbContext.Orders.Add(order);
    await dbContext.SaveChangesAsync(cancellationToken);
    return CreatedAtAction(nameof(GetAsync), new { id = order.Id }, order);
}

// GOOD: Controller delegates to application layer
[HttpPost]
public async Task<ActionResult<OrderResponse>> CreateAsync(CreateOrderHttpRequest request, CancellationToken cancellationToken)
{
    OrderResponse created = await handler.HandleAsync(new CreateOrderRequest(request.CustomerName), cancellationToken);
    return CreatedAtAction(nameof(GetAsync), new { id = created.Id }, created);
}
```

---

## Abstraction Quality

### Good Abstractions
- Represent a real boundary such as persistence, messaging, time, or external identity
- Simplify testing or portability
- Hide vendor-specific concerns from higher layers

### Weak Abstractions
- Generic repositories with many unused methods
- Marker interfaces with no behavior
- Services that only forward one call to another class
- Layers that exist only to repackage the same object graph

### Evaluation Questions
- Does this abstraction isolate a real change point?
- Does it simplify the application layer?
- Would removing it make the system clearer?

---

## Cross-Cutting Concerns

Keep these at the edges unless there is a strong domain reason otherwise:

- Logging
- Authorization wiring
- Caching
- Retry policies
- HTTP client configuration
- Azure SDK configuration

Prefer decorators, middleware, or composition-root wiring over mixing them into domain behavior.

---

## Architecture Review Checklist

### 1. Dependencies
- [ ] Project references point inward only
- [ ] Domain does not reference Infrastructure or WebApi
- [ ] Application does not depend on vendor SDKs or transport types

### 2. Layering
- [ ] Domain contains business rules
- [ ] Application contains orchestration and contracts
- [ ] Infrastructure contains implementation details
- [ ] WebApi remains transport-only

### 3. Abstractions
- [ ] Interfaces represent real boundaries
- [ ] No generic repository by default
- [ ] No pass-through services without value

### 4. Operational Shape
- [ ] Configuration is strongly typed
- [ ] Logging and health checks stay near the edge
- [ ] Security wiring is centralized

### 5. Change Readiness
- [ ] The codebase can evolve without widespread layer leakage
- [ ] Replacing a storage or cloud implementation does not affect Domain
- [ ] Public contracts are not coupled to persistence models

---

## Token Optimization

For large solutions:
1. Inspect `.sln` and `.csproj` files first.
2. Review composition root and dependency registration once.
3. Sample one controller, one application service, and one infrastructure implementation.
4. Search for forbidden references or framework leakage.

Suggested commands:

```bash
rg "<ProjectReference" -g"*.csproj"
rg "using Microsoft\\.EntityFrameworkCore|Azure\\.|HttpContext|ControllerBase" src/Application src/Domain
rg "using .*Infrastructure" src/Domain src/Application
rg "DbContext|BlobServiceClient|ServiceBusClient" src/Application src/Domain
```
