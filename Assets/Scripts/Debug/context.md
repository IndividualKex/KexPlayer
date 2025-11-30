# Debug

Test utilities for verifying networked player systems.

## Purpose

- Input lock timer to test buffered input across network latency
- Interaction event consumption for testing

## Layout

```
Debug/
├── context.md
├── Components/
│   └── InputLockTimer.cs  # Timer component (replicated)
├── Systems/
│   ├── InputLockTimerSystem.cs  # Disables capabilities while timer > 0
│   └── DebugInteractionSystem.cs  # Consumes InteractEvents, sets timer
└── Authoring/
    └── InputLockTimerAuthoring.cs  # Add to player prefab
```

## Dependencies

- KexPlayer (PlayerCapabilities)
- KexInteract (InteractEvent)
- Unity.NetCode
