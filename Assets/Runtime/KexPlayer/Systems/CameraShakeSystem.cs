using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace KexPlayer {
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [BurstCompile]
    public partial struct CameraShakeSystem : ISystem {
        private const float FREQUENCY = 20f;

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var shake in SystemAPI.Query<RefRW<CameraShake>>()) {
                ref var shakeRef = ref shake.ValueRW;

                if (shakeRef.RemainingTime > 0f) {
                    shakeRef.RemainingTime -= SystemAPI.Time.DeltaTime;

                    float intensity = shakeRef.RemainingTime / shakeRef.Duration;
                    float currentMagnitude = shakeRef.Magnitude * intensity * intensity;

                    if (shakeRef.RandomSeed == 0) {
                        shakeRef.RandomSeed = (uint)(SystemAPI.Time.ElapsedTime * 1000) % 65536;
                    }

                    float time = (float)SystemAPI.Time.ElapsedTime;
                    float seed = shakeRef.RandomSeed * 0.01f;

                    shakeRef.Offset = new float3(
                        noise.cnoise(new float2(time * FREQUENCY + seed, 0)) * currentMagnitude,
                        noise.cnoise(new float2(time * FREQUENCY + seed + 100, 0)) * currentMagnitude,
                        noise.cnoise(new float2(time * FREQUENCY + seed + 200, 0)) * currentMagnitude
                    );
                }
                else {
                    shakeRef.Offset = float3.zero;
                }
            }
        }
    }
}
