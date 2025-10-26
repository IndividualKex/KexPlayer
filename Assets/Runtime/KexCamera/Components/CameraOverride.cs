using Unity.Entities;
using Unity.Mathematics;

namespace KexCamera {
    public struct CameraOverride : IComponentData {
        public float3 Position;
        public quaternion Rotation;
        public quaternion OriginalRotation;
        public bool IsActive;
    }
}
