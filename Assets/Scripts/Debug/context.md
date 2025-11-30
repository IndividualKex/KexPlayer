# Debug

Test utilities for verifying networked player systems.

## Layout

```
Debug/
├── context.md
├── FrameRateDebugger.cs  # Frame rate display
└── Systems/
    └── DebugInteractionSystem.cs  # Consumes InteractEvents, sets InputLockTimer
```

## Dependencies

- KexPlayer (InputLockTimer)
- KexInteract (InteractEvent)
