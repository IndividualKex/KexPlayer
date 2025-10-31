using Unity.Entities;

namespace KexInteract {
    public struct Interacter : IComponentData {
        public Entity Target;
        public uint LayerMask;
        public ulong Mask;
    }
}
