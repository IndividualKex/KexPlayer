using Unity.Entities;
using Unity.Mathematics;

namespace KexInteract {
    public struct InteractEvent : IComponentData {
        public Entity Target;
        public Entity Sender;
        public byte Interaction;
        public float3 HitPosition;
    }
}
