using Unity.Entities;

namespace KexCharacter {
    public struct CharacterState : IComponentData {
        public double LastGroundedTime;
    }
}
