# Netcode (Project-Level)

## Purpose

Project-level multiplayer infrastructure for KexPlayer. Handles client-server connection, player spawning, and world management. Project-specific implementation; plugins remain reusable.

## Layout

```
Netcode/
├── context.md
├── Bootstrap/
│   └── AutoConnectBootstrap.cs  # Client-server world creation, auto-connect on port 7979
├── Components/
│   ├── ClientConnectionRequest.cs  # RPC for client join requests
│   └── PlayerConfig.cs  # Singleton with player prefab reference
├── Authoring/
│   └── PlayerConfigAuthoring.cs  # Editor authoring for PlayerConfig singleton
└── Systems/
    ├── ClientInitSystem.cs  # Sends join request, enables camera for local player
    └── ServerInitSystem.cs  # Receives join requests, spawns player entities
```

## Scope

- **In-scope**: Bootstrap, connection management, player spawning, local/multiplayer mode support, owner tagging
- **Out-of-scope**: Input implementation (handled in KexInput), prediction tuning (future work)

## Entrypoints

- **AutoConnectBootstrap**: Called by Unity on play, creates client/server worlds if netcode enabled
- **ClientInitSystem**: Runs on client connect, sends join request when connection ready
- **ServerInitSystem**: Runs on server, spawns player when join request received

## Dependencies

- Unity.NetCode (1.8.0)
- KexPlayer modules (input, camera, character, player)
