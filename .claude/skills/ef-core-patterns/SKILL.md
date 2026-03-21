---
name: ef-core-patterns
description: Use EF Core safely with explicit configurations, migrations, concurrency control, bounded queries, and clear transaction behavior. Use when designing persistence models, repositories, DbContext usage, or migration workflows.
---

# EF Core Patterns Skill

Use EF Core in a way that is explicit, testable, and operationally safe.

## When to Use
- Adding or changing entities
- Designing DbContext boundaries
- Reviewing repository usage
- Writing migrations
- Tuning query or transaction behavior

---

## Quick Reference: Common Issues

| Issue | Symptom | Impact |
|-------|---------|--------|
| Attribute-heavy entities | Mapping spread across domain classes | Persistence concerns leak into core model |
| Unbounded queries | `ToListAsync()` without limits on large collections | Memory and latency problems |
| Missing concurrency token | Silent last-write-wins updates | Lost data under concurrent edits |
| Generic repository overuse | Repository duplicates `DbContext` blindly | More code, less clarity |
| Manual schema changes | Database changed outside migrations | Drift between code and schema |

---

## Entity Mapping

### Prefer Configuration Classes

```csharp
// GOOD: Separate persistence mapping from domain model
public sealed class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.ToTable("WorkItems");
        builder.HasKey(workItem => workItem.Id);
        builder.Property(workItem => workItem.Title).HasMaxLength(200).IsRequired();
        builder.Property(workItem => workItem.RowVersion).IsRowVersion();
    }
}
```

Keep mapping concerns in Infrastructure, not in Domain attributes unless there is a strong reason.

---

## Query Patterns

### Read-Only Queries

```csharp
// GOOD: Read-only query with no tracking and bounded results
IReadOnlyList<WorkItem> items = await dbContext.WorkItems
    .AsNoTracking()
    .OrderByDescending(workItem => workItem.CreatedUtc)
    .Skip(skip)
    .Take(take)
    .ToListAsync(cancellationToken);
```

### Avoid Unbounded Reads

```csharp
// BAD: Unbounded query
List<WorkItem> items = await dbContext.WorkItems.ToListAsync(cancellationToken);
```

---

## Concurrency

Use optimistic concurrency for aggregates that can be updated concurrently.

```csharp
public byte[] RowVersion { get; private set; } = [];

builder.Property(workItem => workItem.RowVersion)
    .IsRowVersion();
```

Handle `DbUpdateConcurrencyException` explicitly where user-facing conflict behavior matters.

---

## Transactions

Use explicit transactions when multiple writes or side effects must succeed together.

```csharp
await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

// multiple writes here

await dbContext.SaveChangesAsync(cancellationToken);
await transaction.CommitAsync(cancellationToken);
```

Do not add explicit transactions around single `SaveChangesAsync` unless there is a real need.

---

## Repository Guidance

### Use a Repository When
- The application layer needs a meaningful persistence boundary
- Query logic needs to be named and reused
- Testing benefits from isolating persistence behavior

### Skip a Repository When
- It only mirrors `DbSet<T>`
- It adds no business-level language
- Direct `DbContext` use in Infrastructure remains clearer

---

## Migration Workflow

- Every schema change should come from an EF Core migration
- Keep migrations in Infrastructure
- Do not manually update schema in ways the model cannot reproduce

Suggested commands:

```bash
dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/WebApi --output-dir Persistence/Migrations
dotnet ef database update --project src/Infrastructure --startup-project src/WebApi
```

---

## Testing Guidance

- Use SQLite for lightweight integration coverage
- Use Testcontainers when provider-specific behavior matters
- Verify mappings, migrations, query behavior, and concurrency-sensitive flows

---

## EF Core Review Checklist

### 1. Mapping
- [ ] Configuration is explicit
- [ ] Persistence concerns stay out of Domain where possible
- [ ] Required fields, lengths, and indexes are intentional

### 2. Query Behavior
- [ ] Read-only queries use `AsNoTracking()` where appropriate
- [ ] Pagination is bounded
- [ ] Includes and projections are deliberate

### 3. Consistency
- [ ] Concurrency control exists where needed
- [ ] Transactions are explicit when necessary
- [ ] SaveChanges boundaries are understandable

### 4. Lifecycle
- [ ] Migrations are used for schema changes
- [ ] Design-time tooling is wired when needed
- [ ] Test coverage exists for important persistence behavior

---

## Token Optimization

For large persistence layers:
1. Read `DbContext` and configuration classes first.
2. Sample the most complex repository or query path.
3. Check migration folders only if schema behavior is relevant.

Suggested commands:

```bash
rg "DbContext|IEntityTypeConfiguration|IsRowVersion|AsNoTracking|BeginTransaction" src/Infrastructure
rg "migrations add|database update|EnsureCreated|EnsureDeleted" src tests
```
