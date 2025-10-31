using Unity.Entities;
using Unity.Mathematics;

namespace KexInteract {
    public struct InteractForce : IComponentData {
        public float3 Force;
    }
}
