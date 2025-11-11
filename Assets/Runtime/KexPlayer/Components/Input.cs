using Unity.Mathematics;
using Unity.NetCode;

namespace KexPlayer {
    public struct Input : IInputComponentData {
        public InputEvent Jump;
        public InputEvent Crouch;
        public InputEvent Sprint;

        public InputEvent Fire;
        public InputEvent AltFire;

        public InputEvent Interact;
        public InputEvent AltInteract;

        public InputEvent Action1;
        public InputEvent Action2;

        public InputEvent Menu;

        public float2 Move;
        public float ViewYawDegrees;
        public float ViewPitchDegrees;
        public int ScrollDelta;
    }
}
