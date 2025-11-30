using Unity.Entities;
using Unity.NetCode;

namespace KexPlayer {
    [GhostComponent]
    public struct InputLockTimer : IComponentData {
        [GhostField] public NetworkTick LockTick;
        [GhostField] public uint LockDurationTicks;

        public bool IsLocked(NetworkTick currentTick) =>
            LockTick.IsValid && currentTick.TicksSince(LockTick) < LockDurationTicks;
    }
}
