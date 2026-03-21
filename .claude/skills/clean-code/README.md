# Clean Code

Human-facing guide for the `clean-code` skill.

## Purpose

Use this skill when you want Claude Code to simplify C# code, improve naming, reduce duplication, and make the result easier to maintain.

## When To Use It

- Refactoring large methods
- Renaming unclear types, methods, or variables
- Removing duplicated logic
- Simplifying controller, service, or handler code
- Reviewing maintainability before merging

## Quick Reference

| Principle | Goal |
|-----------|------|
| DRY | One source of truth |
| KISS | Prefer the simplest working design |
| YAGNI | Avoid speculative abstractions |

## Review Checklist

- Names reveal intent
- Methods are small and focused
- Nesting stays shallow
- Comments explain why, not what
- Extra abstractions are justified

## Typical Prompt

```text
Use the clean-code skill to refactor this service and reduce duplication.
```
