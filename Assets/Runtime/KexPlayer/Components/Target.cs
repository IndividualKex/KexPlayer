using Unity.Entities;
using Unity.Mathematics;

namespace KexPlayer {
    public struct Target : IComponentData {
        public Entity Value;
        public float3 HitPosition;
    }
}
