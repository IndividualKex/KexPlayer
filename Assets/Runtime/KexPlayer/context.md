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
│   ├── Head.cs  # Links head entity to player entity
│   ├── Camera.cs  # Camera data (pitch, sensitivity, eye offset, position, rotation)
│   ├── CameraShake.cs  # Shake effect state
│   ├── CameraOverride.cs  # Override position/rotation with restoration
│   ├── CharacterConfig.cs  # Immutable movement configuration
│   └── CharacterState.cs  # Runtime character state
├── Systems/
│   ├── InputSystem.cs  # GhostInputSystemGroup, captures keyboard/mouse input
│   ├── CameraSystem.cs  # Camera positioning logic
│   ├── CameraShakeSystem.cs  # Shake offset calculation
│   ├── CameraApplySystem.cs  # Apply to Unity Camera.main
│   ├── CharacterPhysicsSystem.cs  # Movement physics (PredictedFixedStepSimulationSystemGroup)
│   ├── CharacterVariableUpdateSystem.cs  # Rotation update (PredictedSimulationSystemGroup)
│   └── HeadUpdateSystem.cs  # Head rotation tracking (PredictedSimulationSystemGroup)
└── Authoring/
    ├── PlayerAuthoring.cs  # Main player authoring with configuration
    ├── PlayerConfigAuthoring.cs  # Singleton with player prefab reference
    ├── HeadAuthoring.cs  # Links head to player
    ├── CameraBootstrap.cs  # Scene initialization (cursor lock)
    └── CameraOverrideAuthoring.cs  # Scene-placed camera overrides
```

## Scope

- **In-scope**: Complete first-person player controller, input capture, view control, camera positioning, character physics, jumping, multiplayer input replication, camera effects, head rotation tracking
- **Out-of-scope**: Third-person cameras, game-specific interactions, rebinding UI, input buffering, inventory systems

## Entrypoints

- **PlayerAuthoring**: Bakes player entity with all required components (Player, Input, Camera, CharacterConfig, CharacterState)
- **HeadAuthoring**: Bakes head entity with Head component linking to player
- **InputSystem**: Runs in GhostInputSystemGroup, captures keyboard/mouse input for entities with GhostOwnerIsLocal, updates Camera component with view angles
- **CharacterPhysicsSystem**: Reads Input (including ViewYawDegrees) to calculate movement direction relative to camera, updates body yaw when moving
- **CharacterVariableUpdateSystem**: Applies body rotation using CharacterState.BodyYawDegrees
- **HeadUpdateSystem**: Updates head entity rotation to match player view angles
- **CameraSystem**: Updates Camera component position/rotation each frame
- **CameraApplySystem**: Applies Camera position/rotation to Unity Camera.main

## System Flow

1. **Input** (GhostInputSystemGroup): InputSystem captures keyboard/mouse → writes Input.ViewYawDegrees/ViewPitchDegrees → updates Camera.YawDegrees/PitchDegrees
2. **Physics** (PredictedFixedStepSimulationSystemGroup): CharacterPhysicsSystem reads Input → calculates movement relative to camera yaw → updates CharacterState.BodyYawDegrees when moving
3. **Variable Update** (PredictedSimulationSystemGroup): CharacterVariableUpdateSystem reads CharacterState.BodyYawDegrees → applies body rotation, then HeadUpdateSystem updates head rotation to match view
4. **Presentation** (PresentationSystemGroup): CameraShakeSystem → CameraSystem (independent camera rotation) → CameraApplySystem

## Components

- **Player**: Tag component for player entities
- **PlayerConfig**: Singleton holding player prefab Entity reference for spawning (used by game-specific spawn systems)
- **Input**: IInputComponentData with netcode replication (Move, ViewYawDegrees, ViewPitchDegrees, Jump, Crouch, Sprint, Fire, AltFire, Interact, AltInteract, Action1, Action2, Menu, ScrollDelta as int normalized to -1/0/1)
- **Head**: Links head entity to player entity
- **Camera**: Camera state (YawDegrees, PitchDegrees, MinPitch, MaxPitch, LookSensitivity, EyeOffset, Position, Rotation)
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
