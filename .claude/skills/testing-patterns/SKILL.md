---
name: testing-patterns
description: Write readable, deterministic .NET tests with xUnit, FluentAssertions, NSubstitute, and focused integration patterns. Use when adding unit tests, API tests, persistence tests, or improving positive and negative coverage.
---

# Testing Patterns Skill

Write tests that are readable, deterministic, and aligned with real application behavior.

## When to Use
- Adding unit tests for domain or application logic
- Adding integration tests for API endpoints
- Testing EF Core mappings and persistence behavior
- Improving failure-path coverage

---

## Quick Reference: Common Issues

| Issue | Symptom | Impact |
|-------|---------|--------|
| Brittle assertions | Tests depend on exact formatting or ordering unnecessarily | Fragile test suite |
| Over-mocking | Every dependency mocked, including trivial ones | Tests drift from real behavior |
| Missing negative cases | Happy path only | Regressions slip through |
| Non-deterministic setup | Real time, random values, shared state | Flaky tests |
| Incorrect test level | HTTP semantics tested only with unit tests | Gaps in real behavior coverage |

---

## Test Stack Defaults

- xUnit for test execution
- FluentAssertions for intent-revealing assertions
- NSubstitute for substitutes and mocks
- `WebApplicationFactory` for API integration tests
- SQLite or Testcontainers for persistence behavior when needed

---

## Unit Test Patterns

### Focus on Behavior

```csharp
[Fact]
public async Task CreateAsync_ShouldPersistOrder_WhenRequestIsValid()
{
    IOrderRepository repository = Substitute.For<IOrderRepository>();
    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    IClock clock = Substitute.For<IClock>();

    clock.UtcNow.Returns(new DateTimeOffset(2026, 3, 21, 8, 0, 0, TimeSpan.Zero));

    CreateOrderHandler sut = new(repository, unitOfWork, clock);

    OrderResponse result = await sut.HandleAsync(new CreateOrderRequest("Acme"), CancellationToken.None);

    result.CustomerName.Should().Be("Acme");
    await repository.Received(1).AddAsync(Arg.Any<Order>(), CancellationToken.None);
    await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
}
```

### Cover Negative Paths

```csharp
[Fact]
public async Task CreateAsync_ShouldThrow_WhenCustomerNameIsMissing()
{
    CreateOrderHandler sut = new(repository, unitOfWork, clock);

    Func<Task> action = async () => await sut.HandleAsync(new CreateOrderRequest(" "), CancellationToken.None);

    await action.Should().ThrowAsync<ArgumentException>();
}
```

---

## Integration Test Patterns

Use integration tests for:
- HTTP routing and serialization
- Middleware and authentication behavior
- EF Core query and mapping behavior
- Database migration and persistence flows

```csharp
[Fact]
public async Task PostAsync_ShouldReturnCreated_WhenPayloadIsValid()
{
    HttpClient client = factory.CreateClient();

    HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/orders", new CreateOrderHttpRequest("Acme"));

    response.StatusCode.Should().Be(HttpStatusCode.Created);
}
```

---

## Test Design Rules

- Prefer deterministic clocks and identifiers
- Keep setup small and local
- Mock only true boundaries
- Prefer real serialization and HTTP for endpoint tests
- Keep test names behavior-oriented

---

## Testing Checklist

### 1. Coverage Shape
- [ ] Positive path exists
- [ ] Negative path exists
- [ ] Important edge cases are covered

### 2. Readability
- [ ] Test name describes behavior
- [ ] Assertions are focused
- [ ] Setup does not hide core intent

### 3. Determinism
- [ ] No shared mutable state across tests
- [ ] Time and randomness are controlled
- [ ] External dependencies are isolated or containerized

### 4. Layer Fit
- [ ] Domain and application behavior is unit-tested
- [ ] HTTP behavior is integration-tested
- [ ] Persistence behavior is tested at the right level

---

## Token Optimization

For test work:
1. Read the production code path first.
2. Add the smallest test that proves behavior.
3. Prefer extending existing fixtures over inventing a new test harness.

Suggested commands:

```bash
rg "\\[Fact\\]|\\[Theory\\]" tests
rg "WebApplicationFactory|Sqlite|Testcontainers|FluentAssertions|NSubstitute" tests
rg "throw new|Results\\.Problem|NotFound|BadRequest" src
```
