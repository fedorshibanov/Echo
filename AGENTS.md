# Unity Project Instructions

## Project
- Portfolio project focused on clean architecture and professional code quality.
- Prioritize maintainability, readability, and simplicity.
- Avoid overengineering.
— Don’t write a lot of tests unless I ask you to. Your task is to help me with the code.
- Always create git branch for your work. You can do commits in your branch, but newer merge unless I ask you to.

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