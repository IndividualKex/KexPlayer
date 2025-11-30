using Unity.Entities;
using Unity.NetCode;

/// <summary>
/// Persistent component tracking the last debug interaction (hitscan pattern).
/// Written by DebugInteractionSystem when interact input + valid target.
/// </summary>
public struct DebugInteraction : IComponentData {
    public NetworkTick Tick;
}
