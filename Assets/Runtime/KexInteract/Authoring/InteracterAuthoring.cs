using UnityEngine;
using Unity.Entities;

namespace KexInteract {
    public class InteracterAuthoring : MonoBehaviour {
        public LayerMask LayerMask = ~0;
        public InteractionMask InteractionMask = InteractionMask.Layer1;

        private class Baker : Baker<InteracterAuthoring> {
            public override void Bake(InteracterAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Interacter {
                    PhysicsMask = authoring.LayerMask,
                    InteractionMask = authoring.InteractionMask,
                });
            }
        }
    }
}
