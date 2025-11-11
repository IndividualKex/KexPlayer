using Unity.Entities;
using UnityEngine;

namespace KexInteract {
    public struct Interacter : IComponentData {
        public Entity Target;
        public LayerMask PhysicsMask;
        public InteractionMask InteractionMask;
    }
}
