using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace KexPlayer {
    [GhostComponent]
    public struct HeadRotation : IComponentData {
        [GhostField]
        public quaternion LocalRotation;
    }
}
