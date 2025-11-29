using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace KexPlayer {
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(CameraShakeSystem))]
    [BurstCompile]
    public partial struct CameraSystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (camera, entity) in SystemAPI
                .Query<RefRW<Camera>>()
                .WithEntityAccess()
            ) {
                ref var cameraRef = ref camera.ValueRW;

                float3 cameraPosition;
                

                if (cameraRef.OverrideEntity != Entity.Null &&
                    SystemAPI.HasComponent<CameraOverride>(cameraRef.OverrideEntity)
                ) {
                    var cameraOverride = SystemAPI.GetComponent<CameraOverride>(cameraRef.OverrideEntity);
                    if (cameraOverride.IsActive) {
                        cameraPosition = cameraOverride.Position;
                    }
                    else {
                        cameraPosition = GetHeadPosition(ref state, cameraRef.HeadEntity);
                    }
                }
                else {
                    cameraPosition = GetHeadPosition(ref state, cameraRef.HeadEntity);
                }

                quaternion cameraRotation = CalculateRotation(cameraRef.YawDegrees, cameraRef.PitchDegrees);

                if (SystemAPI.HasComponent<CameraShake>(entity)) {
                    var shake = SystemAPI.GetComponent<CameraShake>(entity);
                    cameraPosition += shake.Offset;
                }

                cameraRef.Position = cameraPosition;
                cameraRef.Rotation = cameraRotation;

                break;
            }
        }

        private float3 GetHeadPosition(ref SystemState state, Entity headEntity) {
            if (headEntity == Entity.Null) return float3.zero;
            if (!SystemAPI.HasComponent<LocalToWorld>(headEntity)) return float3.zero;
            return SystemAPI.GetComponent<LocalToWorld>(headEntity).Position;
        }

        private static quaternion CalculateRotation(float yawDegrees, float pitchDegrees) {
            quaternion yawRotation = quaternion.Euler(0f, math.radians(yawDegrees), 0f);
            quaternion pitchRotation = quaternion.AxisAngle(math.right(), math.radians(pitchDegrees));
            return math.mul(yawRotation, pitchRotation);
        }
    }
}
