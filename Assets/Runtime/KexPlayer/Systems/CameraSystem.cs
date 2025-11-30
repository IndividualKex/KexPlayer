using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace KexPlayer {
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(CameraShakeSystem))]
    [BurstCompile]
    public partial struct CameraSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var currentTick = networkTime.ServerTick;

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

                bool inputLocked = SystemAPI.HasComponent<InputLockTimer>(entity)
                    && SystemAPI.GetComponent<InputLockTimer>(entity).IsLocked(currentTick);

                quaternion cameraRotation;
                if (inputLocked && SystemAPI.HasComponent<HeadRotation>(entity)) {
                    var headRotation = SystemAPI.GetComponent<HeadRotation>(entity);
                    var bodyRotation = SystemAPI.HasComponent<LocalTransform>(entity)
                        ? SystemAPI.GetComponent<LocalTransform>(entity).Rotation
                        : quaternion.identity;
                    cameraRotation = math.mul(bodyRotation, headRotation.LocalRotation);

                    ExtractYawPitch(cameraRotation, out float yaw, out float pitch);
                    cameraRef.YawDegrees = yaw;
                    cameraRef.PitchDegrees = pitch;
                }
                else {
                    cameraRotation = CalculateRotation(cameraRef.YawDegrees, cameraRef.PitchDegrees);
                }

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

        private static void ExtractYawPitch(quaternion rotation, out float yawDegrees, out float pitchDegrees) {
            float3 forward = math.mul(rotation, math.forward());
            yawDegrees = math.degrees(math.atan2(forward.x, forward.z));
            pitchDegrees = math.degrees(math.asin(-forward.y));
        }
    }
}
