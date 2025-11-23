using Unity.Entities;
using Unity.Mathematics;

namespace KexPlayer {
    public struct Head : IComponentData {
        public Entity Player;
        public float3 Offset;
    }
}
