using Unity.Entities;
using Unity.NetCode;

namespace KexPlayer {
    [GhostComponent]
    public struct InputLockTimer : IComponentData {
        [GhostField]
        public NetworkTick UnlockTick;

        public bool IsLocked(NetworkTick currentTick) =>
            UnlockTick.IsValid && !currentTick.IsNewerThan(UnlockTick);
    }
}
