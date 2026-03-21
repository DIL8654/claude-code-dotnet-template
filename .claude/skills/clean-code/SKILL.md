---
name: clean-code
description: Prefer small, explicit, readable C# code and pragmatic refactoring. Use for cleanup, maintainability reviews, naming improvements, duplication removal, and simplifying overly complex implementations.
---

# Clean Code Skill

Write C# code that is easier to understand, safer to change, and simpler to test.

## When to Use
- Refactoring large or noisy methods
- Renaming unclear classes, methods, or variables
- Reducing duplication
- Simplifying controller, service, or handler logic
- Reviewing maintainability before merging

---

## Quick Reference: Core Principles

| Principle | Meaning | Warning Sign |
|-----------|---------|--------------|
| DRY | One source of truth for repeated knowledge | Copy-pasted validation or branching |
| KISS | Prefer the simplest solution that solves the problem | Overly abstract or generic code |
| YAGNI | Build what is needed now | Extra extension points with no current use |

---

## Naming

### Good Names
- Reveal intent
- Match business language
- Distinguish concepts clearly

```csharp
// BAD
int d;
string s;
List<Order> list;

// GOOD
int elapsedDays;
string customerEmail;
List<Order> overdueOrders;
```

### Boolean Naming

```csharp
// BAD
bool flag;
bool process;

// GOOD
bool isArchived;
bool hasAccess;
bool shouldRetry;
```

---

## Method Design

### Keep Methods Focused

```csharp
// BAD: Validation, mapping, persistence, and logging mixed together
public async Task<IActionResult> CreateAsync(CreateOrderRequest request)
{
    if (string.IsNullOrWhiteSpace(request.CustomerName))
    {
        return BadRequest();
    }

    logger.LogInformation("Creating order");
    Order order = new(request.CustomerName.Trim(), DateTimeOffset.UtcNow);
    dbContext.Orders.Add(order);
    await dbContext.SaveChangesAsync();
    return Ok(order);
}

// GOOD: Small orchestration, clear responsibilities
public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
{
    Validate(request);
    Order order = Map(request);
    await repository.AddAsync(order, cancellationToken);
    await unitOfWork.SaveChangesAsync(cancellationToken);
    return order.ToResponse();
}
```

### Prefer Early Returns

```csharp
// BAD
if (user is not null)
{
    if (user.IsActive)
    {
        return user;
    }
}

throw new InvalidOperationException();

// GOOD
if (user is null)
{
    throw new InvalidOperationException("User was not found.");
}

if (!user.IsActive)
{
    throw new InvalidOperationException("User is inactive.");
}

return user;
```

---

## Abstractions

### Add an Abstraction Only When It Helps

Good reasons:
- Repeated logic with a stable concept
- Real architectural boundary
- Testability at an external dependency

Bad reasons:
- "We may need this later"
- Framework habit without need
- One implementation and no complexity

```csharp
// BAD: Generic repository with many unused members
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> ListAsync();
    Task AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> CountAsync();
}

// GOOD: Specific boundary for an actual use case
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
}
```

---

## Comments

- Prefer self-explanatory code over comments
- Add comments when intent is not obvious
- Do not narrate the code line-by-line

```csharp
// BAD: Redundant comment
// Increment retry count
retryCount++;

// GOOD: Comment explains why, not what
// Stop retrying once the downstream idempotency window has expired.
retryCount++;
```

---

## Clean Code Checklist

### 1. Readability
- [ ] Names reveal intent
- [ ] Methods are small enough to scan quickly
- [ ] Nesting is shallow

### 2. Simplicity
- [ ] No speculative extensibility
- [ ] No abstraction without a real boundary
- [ ] Standard library features are preferred over custom helpers

### 3. Maintainability
- [ ] Duplicate logic is consolidated where appropriate
- [ ] Side effects are easy to spot
- [ ] Error paths are explicit

### 4. Layer Fit
- [ ] Controllers stay thin
- [ ] Business logic stays out of Infrastructure
- [ ] Mapping and validation are not scattered unnecessarily

---

## Token Optimization

For refactors:
1. Read the smallest useful slice first.
2. Search for duplicate logic before generalizing.
3. Fix the highest-noise methods before lower-value style issues.

Suggested commands:

```bash
rg "if \\(|switch \\(|try \\{" src
rg "TODO|FIXME|HACK" src
rg "public .*Controller|public .*Service|public .*Handler" src
```
