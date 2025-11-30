# KexPlayer

First-person player controller for Unity DOTS ECS with input, camera, and character physics.

## Layout

```
KexPlayer/
├── context.md
├── package.json
├── KexPlayer.asmdef
├── Components/
│   ├── Player.cs  # Player tag
│   ├── PlayerConfig.cs  # Singleton with player prefab
│   ├── Input.cs  # IInputComponentData (netcode-replicated)
│   ├── CursorLock.cs  # Client-side cursor lock state
│   ├── InputLockTimer.cs  # Tick-based input lock (NetworkTick UnlockTick)
│   ├── Head.cs  # Links head entity to player
│   ├── HeadRotation.cs  # Replicated head rotation
│   ├── Camera.cs  # Camera state (pitch, yaw, sensitivity)
│   ├── CameraShake.cs  # Shake effect
│   ├── CameraOverride.cs  # Override position/rotation
│   ├── CharacterConfig.cs  # Movement configuration
│   └── CharacterState.cs  # Runtime state (grounded, yaw, jump buffer)
├── Systems/
│   ├── InputSystem.cs  # Cursor lock, input capture (GhostInputSystemGroup)
│   ├── CameraSystem.cs  # Camera positioning
│   ├── CameraShakeSystem.cs  # Shake offset
│   ├── CameraApplySystem.cs  # Apply to Unity Camera.main
│   ├── CharacterPhysicsSystem.cs  # Movement physics (PredictedFixedStepSimulationSystemGroup)
│   ├── CharacterVariableUpdateSystem.cs  # Body rotation
│   ├── HeadUpdateSystem.cs  # Head rotation calculation
│   └── HeadApplySystem.cs  # Apply head rotation to child entity
└── Authoring/
    ├── PlayerAuthoring.cs  # Main player setup
    ├── PlayerConfigAuthoring.cs  # Singleton setup
    ├── HeadAuthoring.cs  # Head entity setup
    └── CameraOverrideAuthoring.cs  # Camera override setup
```

## Key Components

- **InputLockTimer**: Tick-based lock via `IsLocked(NetworkTick)`. Set by external systems to disable movement/look/jump
- **Input**: Netcode-replicated input with Move, View angles, and InputEvents (Jump, Fire, Interact, etc.)

## System Flow

1. **Input** (GhostInputSystemGroup): Cursor lock, keyboard/mouse capture
2. **Physics** (PredictedFixedStepSimulationSystemGroup): Movement relative to camera, respects InputLockTimer
3. **Variable Update** (PredictedSimulationSystemGroup): Body/head rotation
4. **Presentation** (PresentationSystemGroup): Camera shake, positioning, apply to Unity Camera

## Dependencies

- Unity.Entities, Unity.NetCode, Unity.CharacterController, Unity.Physics
