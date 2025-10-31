using UnityEngine;
using Unity.Entities;

namespace KexInteract {
    public class InteracterAuthoring : MonoBehaviour {
        public LayerMask LayerMask = ~0;

        private class Baker : Baker<InteracterAuthoring> {
            public override void Bake(InteracterAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Interacter {
                    LayerMask = (uint)authoring.LayerMask.value
                });
            }
        }
    }
}