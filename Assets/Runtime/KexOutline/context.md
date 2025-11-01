# KexOutline

Visual outline highlighting system for interactable objects in Unity DOTS ECS.

## Purpose

Provides visual feedback to show which entities can be interacted with by rendering them with a special outline effect when targeted by an `Interacter`.

## Architecture

ECS-based system that dynamically switches render layers based on interaction state.

## Components

- `OutlineRenderer`: Tracks the renderer entity and its original layer for an interactable
- `LinkedOutline`: Links child entities to parent interactables for outline propagation
- `OutlineConfig`: Singleton configuration for outline and default layer indices

## Systems

- `OutlineSystem`: Monitors entities with `Interacter` components and switches render layers for targeted interactables

## How It Works

1. Queries all entities with `Interacter` to find current interaction targets
2. For targeted entities, switches their renderer to the outline layer
3. For non-targeted entities, restores their original layer
4. Supports linked child entities via `LinkedOutline`

## Configuration

Initialize `OutlineConfig` singleton at game startup:

```csharp
var config = EntityManager.CreateSingleton(new OutlineConfig {
    OutlineLayer = 13,
    DefaultLayer = 0
});
```

## Dependencies

- KexInteract: Uses `Interactable` and `Interacter` components
- Unity.Entities.Graphics: Uses `RenderFilterSettings` for layer switching

## In Scope

- Visual highlighting logic
- Render layer management
- Linked entity support

## Out of Scope

- Shader/material implementation (provided as sample)
- Interaction logic (handled by KexInteract)
- Game-specific player detection
