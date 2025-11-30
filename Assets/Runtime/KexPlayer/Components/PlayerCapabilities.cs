using System;
using Unity.Entities;
using Unity.NetCode;

namespace KexPlayer {
    [Flags]
    public enum CapabilityFlags : byte {
        None = 0,
        Move = 1 << 0,
        Look = 1 << 1,
        Jump = 1 << 2,
        All = Move | Look | Jump
    }

    [GhostComponent]
    public struct PlayerCapabilities : IComponentData {
        [GhostField]
        public CapabilityFlags Flags;

        public bool CanMove => (Flags & CapabilityFlags.Move) != 0;
        public bool CanLook => (Flags & CapabilityFlags.Look) != 0;
        public bool CanJump => (Flags & CapabilityFlags.Jump) != 0;
    }
}
