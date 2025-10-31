# KexInteract - Generic Interaction System

## Purpose

Generic raycast-based interaction system for Unity DOTS/ECS. Provides unopinionated interaction detection and event generation that can be consumed by game-specific logic.

## Scope

### In-Scope
- Raycast-based interaction detection using Unity Physics
- Distance and angle-based scoring for selecting best interaction target
- Multiple interaction types per entity (primary, alternate, push, hold)
- Event creation for consumed interactions
- Hover state tracking (Interacter component)
- Configurable raycast parameters (distance, layers, scoring factors)
- Integration with KexInput for button state detection
- Integration with KexCamera for raycast origin/direction
- Interaction blocking via InteractionBlocker tag
- Generic mask-based filtering (bitmask overlap checking)

### Out-of-Scope
- Game-specific interaction logic
- Specific mask meanings or context definitions
- Audio/visual feedback
- Specific button mappings (uses generic button indices)
- UI/HUD updates
- Player state management

## Architecture

### Components

- **Interactable**: Marks entity as interactable with configuration for interaction types and optional mask for filtering
- **Interacter**: Tracks current hover target and optional mask for filtering
- **InteractEvent**: Event component created when interaction occurs
- **InteractionBlocker**: Tag to temporarily disable interactions
- **InteractForce**: Stores force vector for physics interactions
- **InteractionConfig**: Singleton with raycast configuration (layers, distances, scoring)

### Systems

- **InteractionSystem**: Main system that raycasts, scores, and creates InteractEvents
- **InteractEventCleanupSystem**: Destroys InteractEvent entities at end of frame

## Design Principles

- **Unopinionated**: No game-specific logic or assumptions
- **Configurable**: All parameters exposed via InteractionConfig singleton
- **Event-driven**: Interaction logic consumes events rather than polling
- **Composable**: Game systems layer on top for specific behaviors
- **Burst-compatible**: All systems support Burst compilation

## Integration

Game code should:
1. Initialize InteractionConfig singleton with layer masks and tuning parameters
2. Add Interacter component to entities that can interact
3. Add Interactable component to entities that can be interacted with
4. Create systems that query for InteractEvent and implement game-specific responses
5. Map button indices (0, 1, 2...) to game-specific ButtonAction enums

### Optional Mask Filtering

Both `Interactable` and `Interacter` components have a `ulong Mask` field for generic filtering:
- If both masks are non-zero, an interaction only occurs if `(interactable.Mask & interacter.Mask) != 0`
- If either mask is zero, no filtering is applied (backward compatible)
- Game code defines what each bit means (e.g., player states, game modes, contexts)

## Dependencies

- Unity.Entities (DOTS)
- Unity.Physics (raycasting)
- Unity.Mathematics
- Unity.Transforms
- KexInput (button state detection)
- KexCamera (raycast origin/direction)
- KexCharacter (kinematic character physics timing)
