# Design Patterns

Human-facing guide for the `design-patterns` skill.

## Purpose

Use this skill when a design problem may benefit from a known pattern, but you want Claude Code to apply it pragmatically instead of overengineering.

## When To Use It

- Replacing conditional-heavy logic
- Encapsulating policy variations
- Improving object creation logic
- Reviewing whether an abstraction is justified

## Quick Reference

| Pattern | Use When |
|---------|----------|
| Strategy | Behavior varies by policy or scenario |
| Factory Method | Construction has real branching |
| Decorator | Cross-cutting behavior wraps a boundary |
| Specification | Query or rule composition is getting noisy |

## Review Checklist

- The pattern solves a current problem
- Added abstractions are justified
- The result is easier to test
- The pattern fits .NET DI and layering cleanly

## Typical Prompt

```text
Use the design-patterns skill to simplify this branching logic.
```
