# Unity Project Instructions

## Project
- Portfolio project focused on clean architecture and professional code quality.
- Prioritize maintainability, readability, and simplicity.
- Avoid overengineering.

## Architecture
- Follow SOLID principles pragmatically.
- Prefer composition over inheritance.
- Separate gameplay logic from Unity-specific code where practical.
- Avoid unnecessary abstractions, singletons, and global state.

## Dependencies
- Use VContainer for dependency injection.
- Prefer constructor injection for plain C# classes.
- Use supported injection mechanisms for MonoBehaviours.
- Avoid service locators and manual dependency resolution.
- Use DOTween for animations and tweening.

## Unity
- Keep MonoBehaviours focused and lightweight.
- Prefer event-driven communication over polling.
- Properly manage subscriptions, tweens, and lifetimes.
- Follow existing project conventions.

## Performance
- Avoid premature optimization.
- Profile before optimizing.
- Minimize allocations and expensive operations in hot paths.