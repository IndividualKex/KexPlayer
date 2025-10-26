# KexInput

Reusable ECS input system for first/third person games.

## Purpose

- Keyboard/mouse input capture via Unity Input System
- Button state tracking (pressed, released, held)
- ECS component-based input distribution

## Layout

```
KexInput/
├── context.md  # This file
├── KexInput.asmdef  # Assembly (references Unity.Entities, Unity.InputSystem)
├── Components/
│   ├── ButtonState.cs  # Button state tracking (IsPressed, WasPressed, WasReleased)
│   └── Input.cs  # Input data (Movement, Look, button states)
└── Systems/
    └── InputSystem.cs  # Input capture and button state management
```

## Scope

- **In-scope**: Input capture (WASD, mouse), button state tracking, ECS integration
- **Out-of-scope**: Game-specific input interpretation, action mapping, rebinding UI

## Entrypoints

- InputSystem updates entities with Input component each frame
- Games query Input component for movement, look, and button states

## Dependencies

- Unity.Entities
- Unity.InputSystem
- Unity.Mathematics
