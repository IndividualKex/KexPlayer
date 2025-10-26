using Unity.Entities;
using Unity.Mathematics;

namespace KexCamera {
    public struct Camera : IComponentData {
        public float PitchDegrees;
        public float MinPitch;
        public float MaxPitch;
        public float LookSensitivity;
        public float3 EyeOffset;
        public Entity OverrideEntity;

        public float3 Position;
        public quaternion Rotation;
    }
}
