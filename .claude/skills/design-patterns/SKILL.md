---
name: design-patterns
description: Apply common design patterns in .NET when they clearly reduce complexity, isolate variation, or improve changeability. Use when evaluating Strategy, Factory, Decorator, Specification, and similar patterns.
---

# Design Patterns Skill

Apply design patterns pragmatically in .NET codebases. Choose the smallest pattern that makes the design clearer.

## When to Use
- Replacing repeated conditional logic
- Isolating policy or algorithm variations
- Simplifying construction with multiple branches
- Reviewing whether an existing abstraction is justified

---

## Quick Reference: Pattern Fit

| Pattern | Use When | Avoid When |
|---------|----------|------------|
| Strategy | Multiple interchangeable behaviors exist | One branch is enough |
| Factory Method | Construction has real branching or hidden dependencies | Constructor call is already clear |
| Decorator | Cross-cutting behavior wraps a boundary | Inheritance would be simpler and local |
| Specification | Query or rule composition is growing noisy | One-off filter logic exists in one place |

---

## Strategy Pattern

```csharp
// GOOD: Policy varies by scenario
public interface IPricingStrategy
{
    decimal Calculate(Order order);
}

public sealed class StandardPricingStrategy : IPricingStrategy
{
    public decimal Calculate(Order order) => order.Subtotal;
}

public sealed class DiscountPricingStrategy : IPricingStrategy
{
    public decimal Calculate(Order order) => order.Subtotal * 0.9m;
}
```

Use when behavior changes by feature flag, product type, tenant, or policy.

---

## Factory Method

```csharp
// GOOD: Construction encapsulates branching
public static class PaymentClientFactory
{
    public static IPaymentClient Create(PaymentProvider provider) =>
        provider switch
        {
            PaymentProvider.Stripe => new StripePaymentClient(),
            PaymentProvider.Adyen => new AdyenPaymentClient(),
            _ => throw new NotSupportedException($"Provider '{provider}' is not supported.")
        };
}
```

Use when object creation depends on configuration or provider choice.

---

## Decorator

```csharp
// GOOD: Add logging around a boundary
public sealed class LoggingOrderPublisher(IOrderPublisher inner, ILogger<LoggingOrderPublisher> logger) : IOrderPublisher
{
    public async Task PublishAsync(OrderCreatedMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing order {OrderId}", message.OrderId);
        await inner.PublishAsync(message, cancellationToken);
    }
}
```

Use for logging, metrics, retries, caching, or authorization around interfaces.

---

## Specification or Query Object

```csharp
// GOOD: Query intent becomes explicit
public sealed record ActiveCustomersQuery(DateOnly? CreatedAfter);
```

Use when repository or query logic starts accumulating many optional filters or rules.

---

## Anti-Patterns

### Generic Repository by Default
- Adds indirection without meaning
- Usually duplicates `DbContext`

### Abstract Factory with One Implementation
- Premature generalization
- More code, less clarity

### Marker Interfaces
- Type hierarchy without behavior
- Often replaced by explicit contracts or attributes

---

## Pattern Selection Checklist

### 1. Problem Fit
- [ ] There is a real repeated or varying behavior
- [ ] The pattern makes the code easier to understand
- [ ] The pattern solves today’s problem, not a hypothetical one

### 2. Cost
- [ ] The extra files and abstractions are justified
- [ ] The pattern reduces branching or coupling
- [ ] The resulting code is easier to test

### 3. .NET Fit
- [ ] DI and composition make the pattern straightforward
- [ ] The pattern does not fight the framework
- [ ] The pattern works with the current layering

---

## Token Optimization

For pattern review:
1. Read the existing conditional or construction logic first.
2. Count current implementations before adding a pattern.
3. Prefer local simplification before introducing new layers.

Suggested commands:

```bash
rg "switch|if .*==|if .* is" src
rg "new .*\\(" src
rg "interface I.*Repository|interface I.*Service" src
```
