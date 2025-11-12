using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace KexInteract {
    public struct Interacter : IComponentData {
        public Entity Target;
        public float3 HitPosition;
        public float InteractDistance;
        public LayerMask PhysicsMask;
        public InteractionMask InteractionMask;
    }
}
