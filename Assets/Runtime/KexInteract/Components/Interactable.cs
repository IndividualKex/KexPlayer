using Unity.Entities;

namespace KexInteract {
    public struct Interactable : IComponentData {
        public InteractionMask InteractionMask;
        public ControlMask ControlMask;
    }
}
