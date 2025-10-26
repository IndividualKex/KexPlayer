using Unity.Entities;
using Unity.Mathematics;

namespace KexInput {
    public struct Input : IComponentData {
        public ButtonState Jump;
        public ButtonState Menu;

        public ButtonState Action1;
        public ButtonState Action2;
        public ButtonState Action3;
        public ButtonState Action4;
        public ButtonState Action5;
        public ButtonState Action6;
        public ButtonState Action7;
        public ButtonState Action8;

        public float2 Move;
        public float2 Look;
        public float ScrollDelta;
    }
}