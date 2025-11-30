using Unity.Entities;
using Unity.NetCode;

[GhostComponent]
public struct InputLockTimer : IComponentData {
    [GhostField]
    public float RemainingTime;
}
