# Project Structure

Unity DOTS (ECS) first-person player controller system with modular input, camera, character physics, and player components

## Stack

- Runtime: Unity 6000.2.6f2
- Language: C# (Unity DOTS/ECS)
- Framework: Unity DOTS (Entities, Physics, Burst, Collections)
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
│   │   ├── KexInput/  # Input handling module
│   │   │   ├── context.md
│   │   │   ├── Components/  # Input components
│   │   │   └── Systems/  # Input systems
│   │   ├── KexCamera/  # Camera module
│   │   │   ├── context.md
│   │   │   ├── Components/  # Camera components
│   │   │   ├── Systems/  # Camera systems
│   │   │   └── Authoring/  # Camera authoring/bootstrap
│   │   ├── KexCharacter/  # Character controller module
│   │   │   ├── context.md
│   │   │   ├── Components/  # Character components
│   │   │   └── Systems/  # Character physics/update systems
│   │   └── KexPlayer/  # Player module
│   │       ├── context.md
│   │       ├── Components/  # Player components
│   │       ├── Systems/  # Player systems
│   │       └── Authoring/  # Player authoring
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
- **Flow**: Input → Character Physics → Camera → Rendering
- **Modules**: KexInput → KexCharacter → KexPlayer → KexCamera

## Entry points

- Bootstrap: Assets/Runtime/KexCamera/Authoring/CameraBootstrap.cs (scene initialization)
- Player Authoring: Assets/Runtime/KexPlayer/Authoring/PlayerAuthoring.cs (player entity setup)
- Input System: Assets/Runtime/KexInput/Systems/InputSystem.cs (input processing entry)
- Character Physics: Assets/Runtime/KexCharacter/Systems/CharacterPhysicsSystem.cs (physics update)

## Naming Conventions

- Files: PascalCase (CharacterState.cs, PlayerSystem.cs)
- Directories: PascalCase (KexPlayer, Components, Systems)
- Namespaces: Match directory name (namespace KexPlayer)
- Components: PascalCase struct implementing IComponentData
- Systems: PascalCase class with "System" suffix, inheriting SystemBase

## Configuration

- Unity Settings: ProjectSettings/ (Unity project configuration)
- Input Actions: Assets/InputSystem_Actions.inputactions (Unity Input System)
- Assembly Definitions: *.asmdef files in each module (compile-time separation)

## Where to add code

- New component data → Assets/Runtime/[Module]/Components/
- New system logic → Assets/Runtime/[Module]/Systems/
- Editor/authoring → Assets/Runtime/[Module]/Authoring/
- New module → Assets/Runtime/[ModuleName]/ (follow existing structure)
