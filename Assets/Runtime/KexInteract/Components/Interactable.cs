using Unity.Entities;

namespace KexInteract {
    public struct Interactable : IComponentData {
        public bool CanInteract;
        public bool CanAltInteract;
        public bool CanPush;
        public bool CanHoldInteract;
        public ulong Mask;

        public bool AllowInteract => CanInteract || CanAltInteract || CanPush || CanHoldInteract;
    }
}
