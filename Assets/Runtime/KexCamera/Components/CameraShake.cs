using Unity.Entities;
using Unity.Mathematics;

namespace KexCamera {
    public struct CameraShake : IComponentData {
        public float3 Offset;
        public float Duration;
        public float RemainingTime;
        public float Magnitude;
        public uint RandomSeed;
    }
}
