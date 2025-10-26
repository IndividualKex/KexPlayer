using Unity.Entities;
using Unity.Mathematics;

namespace KexCharacter {
    public struct CharacterInput : IComponentData {
        public float2 MovementInput;
        public float YawRotationRadians;
        public double JumpRequestTime;
    }
}
