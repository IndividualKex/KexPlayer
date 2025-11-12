# Netcode (Project-Level)

## Purpose

Reference implementation of multiplayer connection flow for KexPlayer. Handles client-server connection and player spawning. Host projects can customize or replace this logic as needed (e.g., authentication, lobbies, spawn points, team assignment).

## Layout

```
Netcode/
├── context.md
├── Bootstrap/
│   └── AutoConnectBootstrap.cs  # Client-server world creation, auto-connect on port 7979
├── Components/
│   └── ClientConnectionRequest.cs  # RPC for client join requests
└── Systems/
    ├── ClientInitSystem.cs  # Sends join request when connected
    └── ServerInitSystem.cs  # Receives join requests, spawns player (uses KexPlayer.PlayerConfig)
```

## Scope

- **In-scope**: Bootstrap configuration, connection flow, basic player spawn logic
- **Out-of-scope**: PlayerConfig component (now in KexPlayer package), authentication, lobbies, spawn points, team assignment (customize as needed)

## Entrypoints

- **AutoConnectBootstrap**: Called by Unity on play, creates client/server worlds if netcode enabled
- **ClientInitSystem**: Runs on client connect, sends join request when connection ready
- **ServerInitSystem**: Runs on server, spawns player when join request received

## Dependencies

- Unity.NetCode (1.8.0)
- KexPlayer modules (input, camera, character, player)
