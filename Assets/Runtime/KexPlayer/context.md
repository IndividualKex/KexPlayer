# KexPlayer

Reusable ECS first-person player controller integrating KexInput, KexCamera, and KexCharacter.

## Purpose

- First-person player controller (look + movement)
- Input → camera pitch and character yaw rotation
- Input → character movement (WASD + jump)
- Drop-in player for Unity DOTS first-person games

## Layout

```
KexPlayer/
├── context.md  # This file
├── KexPlayer.asmdef  # Assembly (references KexInput, KexCamera, KexCharacter)
├── Components/
│   └── Player.cs  # Player state (look sensitivity, original yaw)
├── Systems/
│   └── PlayerSystem.cs  # Input mapping to camera/character
└── Authoring/
    └── PlayerAuthoring.cs  # MonoBehaviour authoring for player setup
```

## Scope

- **In-scope**: Look control (pitch/yaw), movement input mapping, jump input, player authoring
- **Out-of-scope**: Game-specific interactions, context management, footsteps, held items, building systems

## Entrypoints

- PlayerSystem reads KexInput.Input and updates KexCamera.Camera pitch + KexCharacter.CharacterInput
- PlayerAuthoring bakes all required components (Player, Input, Camera, CharacterConfig, CharacterInput, CharacterState)
- Games extend the player entity with additional components for game-specific behavior

## Dependencies

- KexInput (input capture)
- KexCamera (camera control)
- KexCharacter (character physics)
- Unity.Entities
- Unity.Mathematics
- Unity.Transforms
- Unity.CharacterController
