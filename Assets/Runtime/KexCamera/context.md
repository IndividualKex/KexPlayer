# KexCamera

Reusable ECS first-person camera system.

## Purpose

- First-person camera positioning and rotation
- Pitch control (up/down look) with configurable sensitivity
- Optional camera shake effects
- Optional camera overrides

## Layout

```
KexCamera/
├── context.md  # This file
├── KexCamera.asmdef  # Assembly (references Unity.Entities, Unity.Mathematics)
├── Components/
│   ├── Camera.cs  # Main camera data (pitch, sensitivity, eye offset, position, rotation)
│   ├── CameraShake.cs  # Shake effect state
│   └── CameraOverride.cs  # Override position/rotation (stores original rotation for restoration)
├── Systems/
│   ├── CameraShakeSystem.cs  # Shake offset calculation
│   ├── CameraSystem.cs  # Camera positioning logic
│   └── CameraApplySystem.cs  # Apply to Unity Camera.main
└── Authoring/
    └── CameraOverrideAuthoring.cs  # MonoBehaviour for scene-placed camera overrides
```

## Scope

- **In-scope**: First-person camera positioning, pitch rotation, shake effects, overrides
- **Out-of-scope**: Third-person cameras, orbit cameras, input handling (see KexInput)

## Entrypoints

- CameraSystem updates Camera component position/rotation each frame
- CameraApplySystem applies to Unity Camera.main transform
- External systems update Camera.PitchDegrees for looking up/down

## Dependencies

- Unity.Entities
- Unity.Mathematics
- Unity.Transforms
