using Unity.Entities;
using Unity.Mathematics;

namespace KexPlayer {
    public struct CameraOverride : IComponentData {
        public float3 Position;
        public quaternion Rotation;
        public quaternion OriginalRotation;
        public bool IsActive;
    }
}
