using Unity.Entities;
using Unity.Mathematics;

namespace KexPlayer {
    public struct Camera : IComponentData {
        public float YawDegrees;
        public float PitchDegrees;
        public float MinPitch;
        public float MaxPitch;
        public float LookSensitivity;
        public Entity HeadEntity;
        public Entity OverrideEntity;

        public float3 Position;
        public quaternion Rotation;
    }
}
