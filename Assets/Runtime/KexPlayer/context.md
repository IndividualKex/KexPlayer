# KexPlayer

Complete first-person player controller system for Unity DOTS ECS with integrated input, camera, and character physics.

## Purpose

- First-person player controller with keyboard/mouse input
- Independent camera rotation (full 360° yaw/pitch)
- Body rotation follows movement direction
- Character movement physics with jumping and coyote time
- Netcode-native multiplayer support
- Optional camera shake and override effects

## Layout

```
KexPlayer/
├── context.md  # This file
├── package.json  # Unity package manifest
├── KexPlayer.asmdef  # Assembly definition
├── Components/
│   ├── Player.cs  # Player tag component
│   ├── PlayerConfig.cs  # Singleton with player prefab reference (for spawning)
│   ├── Input.cs  # IInputComponentData (netcode-replicated input)
│   ├── CursorLock.cs  # Client-side cursor lock state (bool)
│   ├── PlayerCapabilities.cs  # Capability flags (Move, Look, Jump)
│   ├── Head.cs  # Links head entity to player entity
│   ├── HeadRotation.cs  # Replicated head rotation on player entity
│   ├── Camera.cs  # Camera data (pitch, sensitivity, head entity, position, rotation)
│   ├── CameraShake.cs  # Shake effect state
│   ├── CameraOverride.cs  # Override position/rotation with restoration
│   ├── CharacterConfig.cs  # Immutable movement configuration
│   └── CharacterState.cs  # Runtime character state
├── Systems/
│   ├── InputSystem.cs  # GhostInputSystemGroup, cursor lock handling and input capture
│   ├── CameraSystem.cs  # Camera positioning logic
│   ├── CameraShakeSystem.cs  # Shake offset calculation
│   ├── CameraApplySystem.cs  # Apply to Unity Camera.main
│   ├── CharacterPhysicsSystem.cs  # Movement physics (PredictedFixedStepSimulationSystemGroup)
│   ├── CharacterVariableUpdateSystem.cs  # Rotation update (PredictedSimulationSystemGroup)
│   ├── HeadUpdateSystem.cs  # Calculates head rotation on player entity (PredictedSimulationSystemGroup)
│   └── HeadApplySystem.cs  # Applies head rotation to child entity (TransformSystemGroup)
└── Authoring/
    ├── PlayerAuthoring.cs  # Main player authoring with configuration
    ├── PlayerConfigAuthoring.cs  # Singleton with player prefab reference
    ├── HeadAuthoring.cs  # Links head to player
    └── CameraOverrideAuthoring.cs  # Scene-placed camera overrides
```

## Scope

- **In-scope**: Complete first-person player controller, input capture, view control, camera positioning, character physics, jumping, multiplayer input replication, camera effects, networked head rotation
- **Out-of-scope**: Third-person cameras, game-specific interactions, rebinding UI, input buffering, inventory systems

## Entrypoints

- **PlayerAuthoring**: Bakes player entity with all required components (Player, Input, Camera, CharacterConfig, CharacterState, HeadRotation, CursorLock, PlayerCapabilities). References HeadAuthoring to set Camera.HeadEntity
- **HeadAuthoring**: Bakes head entity with Head component linking to player
- **InputSystem**: Handles cursor lock (Escape/click/focus loss), captures input when locked, preserves view angles when unlocked
- **CharacterPhysicsSystem**: Reads Input (including ViewYawDegrees) to calculate movement direction relative to camera, updates body yaw when moving
- **CharacterVariableUpdateSystem**: Applies body rotation using CharacterState.BodyYawDegrees
- **HeadUpdateSystem**: Calculates head rotation from input, writes to player's HeadRotation component (predicted ghosts only)
- **HeadApplySystem**: Applies HeadRotation from player to child head entity's LocalTransform (all players)
- **CameraSystem**: Updates Camera component position (from head entity) and rotation each frame
- **CameraApplySystem**: Applies Camera position/rotation to Unity Camera.main

## System Flow

1. **Input** (GhostInputSystemGroup): InputSystem handles cursor lock and captures keyboard/mouse when locked → writes Input.ViewYawDegrees/ViewPitchDegrees → updates Camera.YawDegrees/PitchDegrees
2. **Physics** (PredictedFixedStepSimulationSystemGroup): CharacterPhysicsSystem reads Input → calculates movement relative to camera yaw → updates CharacterState.BodyYawDegrees when moving
3. **Variable Update** (PredictedSimulationSystemGroup): CharacterVariableUpdateSystem applies body rotation → HeadUpdateSystem calculates head rotation → writes to HeadRotation component
4. **Transform Update** (SimulationSystemGroup): HeadApplySystem reads HeadRotation → applies to head child entity LocalTransform
5. **Presentation** (PresentationSystemGroup): CameraShakeSystem → CameraSystem (independent camera rotation) → CameraApplySystem

## Components

- **Player**: Tag component for player entities
- **PlayerConfig**: Singleton holding player prefab Entity reference for spawning (used by game-specific spawn systems)
- **Input**: IInputComponentData with netcode replication (Move, ViewYawDegrees, ViewPitchDegrees, Jump, Crouch, Sprint, Fire, AltFire, Interact, AltInteract, Action1, Action2, Menu, ScrollUp, ScrollDown as InputEvents). Set to default when cursor unlocked
- **CursorLock**: Client-side cursor lock state (bool with implicit operators). When false, input passthrough disabled
- **PlayerCapabilities**: Capability flags controlling host-level actions (CapabilityFlags: Move, Look, Jump). External systems write; KexPlayer systems check before processing. Defaults to All
- **Head**: Links head entity to player entity
- **HeadRotation**: Replicated head rotation stored on player entity (LocalRotation as quaternion with [GhostField])
- **Camera**: Camera state (YawDegrees, PitchDegrees, MinPitch, MaxPitch, LookSensitivity, HeadEntity, Position, Rotation)
- **CameraShake**: Optional shake effect (Offset, Duration, RemainingTime, Magnitude, RandomSeed)
- **CameraOverride**: Optional override (Position, Rotation, OriginalRotation, IsActive)
- **CharacterConfig**: Immutable movement config (speeds, gravity, jump height, coyote time, step/slope handling)
- **CharacterState**: Runtime state (LastGroundedTime, BodyYawDegrees)

## Dependencies

- Unity.Entities
- Unity.NetCode
- Unity.InputSystem
- Unity.CharacterController
- Unity.Physics
- Unity.Mathematics
- Unity.Transforms
- Unity.Burst
- Unity.Collections
