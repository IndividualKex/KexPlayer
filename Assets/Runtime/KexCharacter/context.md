# KexCharacter

Reusable ECS first-person character controller for Unity CharacterController-based games.

## Purpose

- First-person character movement (grounded and air)
- Jump mechanics with coyote time
- Physics-based character control via Unity CharacterController
- Input-driven movement and rotation

## Layout

```
KexCharacter/
├── context.md  # This file
├── KexCharacter.asmdef  # Assembly (references Unity.Entities, Unity.CharacterController, Unity.Physics)
├── Components/
│   ├── CharacterConfig.cs  # Immutable movement configuration
│   ├── CharacterState.cs  # Runtime state (managed by systems)
│   └── CharacterInput.cs  # Input commands (written by external systems)
└── Systems/
    ├── CharacterPhysicsSystem.cs  # Movement physics (runs in KinematicCharacterPhysicsUpdateGroup)
    └── CharacterVariableUpdateSystem.cs  # Rotation interpolation (runs in SimulationSystemGroup)
```

## Scope

- **In-scope**: First-person character movement, jumping, rotation, physics control
- **Out-of-scope**: Input handling (see KexInput), camera control (see KexCamera), authoring (see game-specific player plugin)

## Entrypoints

- CharacterPhysicsSystem reads CharacterInput and CharacterConfig to update movement and CharacterState
- CharacterVariableUpdateSystem applies rotation interpolation using CharacterInput
- External systems write to CharacterInput each frame (MovementInput, YawRotationRadians, JumpRequestTime)
- CharacterConfig is set during authoring and remains immutable at runtime
- CharacterState is managed internally by CharacterPhysicsSystem

## Dependencies

- Unity.Entities
- Unity.CharacterController
- Unity.Physics
- Unity.Mathematics
- Unity.Transforms
- Unity.Burst
