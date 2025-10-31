using UnityEngine;
using Unity.Entities;

namespace KexInteract {
    public class InteractableAuthoring : MonoBehaviour {
        public bool CanInteract = true;
        public bool CanAltInteract = false;
        public bool CanPush = false;
        public bool CanHoldInteract = false;
        public ulong Mask;

        public class Baker : Baker<InteractableAuthoring> {
            public override void Bake(InteractableAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new Interactable {
                    CanInteract = authoring.CanInteract,
                    CanAltInteract = authoring.CanAltInteract,
                    CanPush = authoring.CanPush,
                    CanHoldInteract = authoring.CanHoldInteract,
                    Mask = authoring.Mask,
                });
            }
        }
    }
}