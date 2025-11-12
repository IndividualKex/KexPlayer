using Unity.Entities;
using Unity.NetCode;

namespace KexPlayer {
    [GhostComponent]
    public struct CharacterState : IComponentData {
        [GhostField]
        public double LastGroundedTime;
        [GhostField]
        public float BodyYawDegrees;
    }
}
