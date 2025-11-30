# Project Structure

Unity DOTS (ECS) first-person player controller system with integrated input, camera, and character physics

## Stack

- Runtime: Unity 6000.2.6f2
- Language: C# (Unity DOTS/ECS)
- Framework: Unity DOTS (Entities, Physics, Burst, Collections)
- Networking: Unity Netcode for Entities (1.8.0)
- Build: Unity Build System
- Testing: Unity Test Framework
- Version Control: PlasticSCM/Unity Collab

## Commands

- Open: Open project in Unity Editor
- Build: Unity Editor → File → Build Settings → Build
- Play: Unity Editor → Play button (test in-editor)
- Test: Unity Editor → Window → General → Test Runner

## Layout

```
KexPlayer/
├── CLAUDE.md  # Global context (Tier 0)
├── Assets/  # Unity assets
│   ├── Runtime/  # Runtime code (C# scripts)
│   │   ├── KexPlayer/  # Complete player controller (installable package)
│   │   │   ├── context.md
│   │   │   ├── package.json  # Unity package manifest
│   │   │   ├── KexPlayer.asmdef  # Assembly definition
│   │   │   ├── Components/  # All player components
│   │   │   │   ├── Player.cs  # Player tag
│   │   │   │   ├── PlayerConfig.cs  # Singleton with player prefab reference
│   │   │   │   ├── Input.cs  # Input data (netcode-replicated)
│   │   │   │   ├── InputBuffer.cs  # Input buffering timestamps
│   │   │   │   ├── CursorLock.cs  # Cursor lock state
│   │   │   │   ├── PlayerCapabilities.cs  # Capability flags (Move, Look, Jump)
│   │   │   │   ├── Head.cs  # Head entity link
│   │   │   │   ├── Camera.cs  # Camera data
│   │   │   │   ├── CameraShake.cs  # Camera shake
│   │   │   │   ├── CameraOverride.cs  # Camera override
│   │   │   │   ├── CharacterConfig.cs  # Character configuration
│   │   │   │   └── CharacterState.cs  # Character state
│   │   │   ├── Systems/  # All player systems
│   │   │   │   ├── InputSystem.cs  # Input capture and cursor lock
│   │   │   │   ├── CameraSystem.cs  # Camera positioning
│   │   │   │   ├── CameraShakeSystem.cs  # Camera shake
│   │   │   │   ├── CameraApplySystem.cs  # Apply to Unity Camera
│   │   │   │   ├── CharacterPhysicsSystem.cs  # Physics update
│   │   │   │   ├── CharacterVariableUpdateSystem.cs  # Rotation update
│   │   │   │   └── HeadUpdateSystem.cs  # Head rotation tracking
│   │   │   └── Authoring/  # Player authoring
│   │   │       ├── PlayerAuthoring.cs  # Main player setup
│   │   │       ├── PlayerConfigAuthoring.cs  # Singleton with player prefab
│   │   │       ├── HeadAuthoring.cs  # Head entity setup
│   │   │       └── CameraOverrideAuthoring.cs  # Camera overrides
│   │   ├── KexInteract/  # Interaction module (installable package)
│   │   │   ├── context.md
│   │   │   ├── package.json  # Unity package manifest
│   │   │   ├── Enums.cs  # InteractionMask, ControlMask
│   │   │   ├── Components/  # Interaction components
│   │   │   ├── Systems/  # Interaction systems
│   │   │   └── Authoring/  # Interaction authoring
│   │   └── KexOutline/  # Outline rendering module (installable package)
│   │       ├── context.md
│   │       ├── package.json  # Unity package manifest
│   │       ├── Components/  # Outline components
│   │       ├── Systems/  # Outline rendering systems
│   │       └── Authoring/  # Outline authoring
│   ├── Scripts/  # Project-specific code
│   │   └── Netcode/  # Multiplayer infrastructure (reference implementation)
│   │       ├── context.md
│   │       ├── Bootstrap/  # Client-server world creation
│   │       ├── Components/  # ClientConnectionRequest RPC
│   │       └── Systems/  # Connection and spawn systems
│   ├── Prefabs/  # Unity prefabs
│   ├── Scenes/  # Unity scenes
│   ├── Settings/  # Project settings
│   └── InputSystem_Actions.inputactions  # Input actions
├── layers/
│   ├── structure.md  # Project-level context (Tier 1)
│   └── context-template.md  # Template for context files
├── ProjectSettings/  # Unity project configuration
├── Packages/  # Unity package dependencies
└── *.csproj  # C# project files (auto-generated)
```

## Architecture

- **Pattern**: ECS (Entity Component System)
- **Layers**: Components (data) → Systems (logic) → Authoring (editor bindings)
- **Flow**: Input (GhostInputSystemGroup) → Character Physics (PredictedFixedStepSimulationSystemGroup) → Camera (PresentationSystemGroup)
- **Modules**: KexPlayer (complete player controller), KexInteract (requires KexPlayer), KexOutline (requires KexInteract)

## Entry points

- Player Authoring: Assets/Runtime/KexPlayer/Authoring/PlayerAuthoring.cs (player entity setup)
- Input System: Assets/Runtime/KexPlayer/Systems/InputSystem.cs (input capture, cursor lock, view processing)
- Character Physics: Assets/Runtime/KexPlayer/Systems/CharacterPhysicsSystem.cs (physics update)

## Naming Conventions

- Files: PascalCase (CharacterState.cs, InputSystem.cs)
- Directories: PascalCase (KexPlayer, Components, Systems)
- Namespaces: Match directory name (namespace KexPlayer)
- Components: PascalCase struct implementing IComponentData
- Systems: PascalCase class with "System" suffix, inheriting SystemBase

## Configuration

- Unity Settings: ProjectSettings/ (Unity project configuration)
- Input Actions: Assets/InputSystem_Actions.inputactions (Unity Input System)
- Assembly Definitions: *.asmdef files in each module (compile-time separation)
- Package Manifests: package.json in each module (git package distribution, all include netcode 1.8.0)
- Netcode Bootstrap: Assets/Scripts/Netcode/Bootstrap/AutoConnectBootstrap.cs (multiplayer worlds)

## Where to add code

- New component data → Assets/Runtime/[Module]/Components/
- New system logic → Assets/Runtime/[Module]/Systems/
- Editor/authoring → Assets/Runtime/[Module]/Authoring/
- New module → Assets/Runtime/[ModuleName]/ (follow existing structure, include package.json)

## Package Distribution

Each module (KexPlayer, KexInteract, KexOutline) is an installable Unity package via git URL:
- Install via Package Manager: `https://github.com/IndividualKex/KexPlayer.git?path=Assets/Runtime/[Module]`
- Or add to Packages/manifest.json
- All packages include Unity Netcode for Entities (1.8.0) dependency
- KexPlayer is a complete player controller with input, camera, and character physics (v0.2.0)
- KexInteract requires KexPlayer (manual installation, Unity doesn't support git dependencies between packages)
- KexOutline requires KexInteract (manual installation)
- Multiplayer support requires project-level netcode setup (Assets/Scripts/Netcode/)
