using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace KexInteract {
    public struct InteractEvent : IComponentData {
        public Entity Target;
        public Entity Sender;
        public float3 HitPosition;
        public byte Interaction;
        public NetworkTick Tick;
    }
}
