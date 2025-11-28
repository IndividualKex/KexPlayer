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

        public InputEvent Digit1;
        public InputEvent Digit2;
        public InputEvent Digit3;
        public InputEvent Digit4;
        public InputEvent Digit5;
        public InputEvent Digit6;
        public InputEvent Digit7;
        public InputEvent Digit8;
        public InputEvent Digit9;
        public InputEvent Digit0;

        public float2 Move;
        public float ViewYawDegrees;
        public float ViewPitchDegrees;
        public int ScrollDelta;
    }
}
