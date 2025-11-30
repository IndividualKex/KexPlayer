# KexInteract - Interaction System

## Purpose

Raycast-based interaction system for Unity DOTS/ECS with control-specific masking and event generation.

## Scope

### In-Scope
- Raycast-based targeting with distance/angle scoring
- Control-specific interaction filtering (Fire, AltFire, Interact, AltInteract, Action1, Action2)
- InteractionMask filtering between Interacter and Interactable
- Event creation for interactions
- Target tracking in Interacter component
- Interaction blocking via InteractionBlocker tag

### Out-of-Scope
- Game-specific interaction responses
- Audio/visual feedback
- UI/HUD updates

## Architecture

### Components

- **Interactable**: InteractionMask (filtering) + ControlMask (allowed controls)
- **Interacter**: Target, HitPosition, InteractDistance, PhysicsMask, InteractionMask
- **InteractEvent**: Target, Sender, Interaction (byte index), HitPosition
- **InteractionBlocker**: Tag to disable interactions

### Systems

- **InteractTargetingSystem**: Raycasts, scores, updates Interacter.Target (PredictedFixedStepSimulationSystemGroup)
- **InteractSystem**: Reads Input, checks ControlMask, creates InteractEvents; skips entities with InteractionBlocker (PredictedSimulationSystemGroup)
- **InteractEventCleanupSystem**: Destroys InteractEvent entities

## Integration

1. Add Interacter to player (set InteractDistance, PhysicsMask, optional InteractionMask)
2. Add Interactable to interactive entities (set InteractionMask, ControlMask)
3. Create systems that query InteractEvent and implement responses (access HitPosition for world-space interaction point)

## Dependencies

- Unity.Entities
- Unity.Physics
- Unity.Mathematics
- Unity.Transforms
- Unity.NetCode
- KexPlayer (Input, Camera)
