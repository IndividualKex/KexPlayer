# KexPlayer

Complete first-person player controller system for Unity DOTS ECS with integrated input, camera, and character physics.

## Purpose

- First-person player controller with keyboard/mouse input
- Camera positioning and rotation with pitch control
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
│   ├── Input.cs  # IInputComponentData (netcode-replicated input)
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
│   └── CharacterVariableUpdateSystem.cs  # Rotation update (PredictedSimulationSystemGroup)
└── Authoring/
    ├── PlayerAuthoring.cs  # Main player authoring with configuration
    ├── CameraBootstrap.cs  # Scene initialization (cursor lock)
    └── CameraOverrideAuthoring.cs  # Scene-placed camera overrides
```

## Scope

- **In-scope**: Complete first-person player controller, input capture, view control, camera positioning, character physics, jumping, multiplayer input replication, camera effects
- **Out-of-scope**: Third-person cameras, game-specific interactions, rebinding UI, input buffering, inventory systems

## Entrypoints

- **PlayerAuthoring**: Bakes player entity with all required components (Player, Input, Camera, CharacterConfig, CharacterState)
- **InputSystem**: Runs in GhostInputSystemGroup, captures keyboard/mouse input for entities with GhostOwnerIsLocal, updates Camera component with view angles
- **CharacterPhysicsSystem**: Reads Input component and CharacterConfig to update movement physics and CharacterState
- **CharacterVariableUpdateSystem**: Applies rotation using Input.ViewYawDegrees
- **CameraSystem**: Updates Camera component position/rotation each frame
- **CameraApplySystem**: Applies Camera position/rotation to Unity Camera.main

## System Flow

1. **Input** (GhostInputSystemGroup): InputSystem captures keyboard/mouse → writes to Input component → updates Camera.YawDegrees/PitchDegrees
2. **Physics** (PredictedFixedStepSimulationSystemGroup): CharacterPhysicsSystem reads Input → applies movement/jumping → updates CharacterState
3. **Variable Update** (PredictedSimulationSystemGroup): CharacterVariableUpdateSystem reads Input.ViewYawDegrees → applies rotation
4. **Presentation** (PresentationSystemGroup): CameraShakeSystem → CameraSystem → CameraApplySystem

## Components

- **Player**: Tag component for player entities
- **Input**: IInputComponentData with netcode replication (Move, ViewYawDegrees, ViewPitchDegrees, Jump, Crouch, Sprint, Fire, AltFire, Interact, AltInteract, Action1, Action2, Menu, ScrollDelta as int normalized to -1/0/1)
- **Camera**: Camera state (YawDegrees, PitchDegrees, MinPitch, MaxPitch, LookSensitivity, EyeOffset, Position, Rotation)
- **CameraShake**: Optional shake effect (Offset, Duration, RemainingTime, Magnitude, RandomSeed)
- **CameraOverride**: Optional override (Position, Rotation, OriginalRotation, IsActive)
- **CharacterConfig**: Immutable movement config (speeds, gravity, jump height, coyote time, step/slope handling)
- **CharacterState**: Runtime state (LastGroundedTime)

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
