using Unity.Entities;

namespace KexPlayer {
    public struct TargetingConfig : IComponentData {
        public float Distance;
        public uint PhysicsLayerMask;
    }
}
