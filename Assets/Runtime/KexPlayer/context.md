# KexPlayer

First-person player controller for Unity DOTS ECS with input, camera, character physics, and targeting.

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
│   ├── InputLockTimer.cs  # Tick-based input lock (ghost component)
│   ├── Target.cs  # Current targeting result (client-only)
│   ├── TargetingConfig.cs  # Targeting distance and layer mask
│   ├── Head.cs  # Links head entity to player
│   ├── HeadRotation.cs  # Replicated head rotation
│   ├── Camera.cs  # Camera state (pitch, yaw, sensitivity)
│   ├── CameraShake.cs  # Shake effect
│   ├── CameraOverride.cs  # Override position/rotation
│   ├── CharacterConfig.cs  # Movement configuration
│   └── CharacterState.cs  # Runtime state (grounded, yaw, jump buffer)
├── Systems/
│   ├── InputSystem.cs  # Cursor lock, input capture (GhostInputSystemGroup)
│   ├── TargetingSystem.cs  # Raycast targeting (FixedStepSimulationSystemGroup)
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

- **InputLockTimer**: Ghost component with `IsLocked(NetworkTick)`. Set by external systems to disable movement
- **Input**: Netcode-replicated input with Move, View angles, and InputEvents (Jump, Fire, Interact, etc.)
- **Target**: Client-only targeting result from raycast (not synced for prediction simplicity)

## System Flow

1. **Input** (GhostInputSystemGroup): Cursor lock, keyboard/mouse capture
2. **Physics** (PredictedFixedStepSimulationSystemGroup): Movement relative to camera, respects InputLockTimer
3. **Targeting** (FixedStepSimulationSystemGroup): Raycast from head, updates Target
4. **Variable Update** (PredictedSimulationSystemGroup): Body/head rotation
5. **Presentation** (PresentationSystemGroup): Camera shake, positioning, apply to Unity Camera

## Dependencies

- Unity.Entities, Unity.NetCode, Unity.CharacterController, Unity.Physics
