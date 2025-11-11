using UnityEngine;
using Unity.Entities;

namespace KexInteract {
    public class InteractableAuthoring : MonoBehaviour {
        public InteractionMask InteractionMask = InteractionMask.Layer1;
        public ControlMask ControlMask = (ControlMask)byte.MaxValue;

        public class Baker : Baker<InteractableAuthoring> {
            public override void Bake(InteractableAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new Interactable {
                    InteractionMask = authoring.InteractionMask,
                    ControlMask = authoring.ControlMask,
                });
            }
        }
    }
}
