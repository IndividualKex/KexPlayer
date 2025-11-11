using Unity.Entities;

namespace KexInteract {
    public struct InteractEvent : IComponentData {
        public Entity Target;
        public Entity Sender;
        public byte Interaction;
    }
}
