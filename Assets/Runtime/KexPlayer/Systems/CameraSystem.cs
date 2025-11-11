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
            foreach (var (camera, localToWorld, entity) in SystemAPI
                .Query<RefRW<Camera>, LocalToWorld>()
                .WithEntityAccess()
            ) {
                ref var cameraRef = ref camera.ValueRW;

                float3 entityPosition = localToWorld.Position;
                quaternion entityRotation = localToWorld.Rotation;

                float3 cameraPosition;

                if (cameraRef.OverrideEntity != Entity.Null &&
                    SystemAPI.HasComponent<CameraOverride>(cameraRef.OverrideEntity)
                ) {
                    var cameraOverride = SystemAPI.GetComponent<CameraOverride>(cameraRef.OverrideEntity);
                    if (cameraOverride.IsActive) {
                        cameraPosition = cameraOverride.Position;
                    }
                    else {
                        cameraPosition = CalculatePosition(entityPosition, entityRotation, cameraRef.EyeOffset);
                    }
                }
                else {
                    cameraPosition = CalculatePosition(entityPosition, entityRotation, cameraRef.EyeOffset);
                }

                quaternion cameraRotation = CalculateRotation(entityRotation, cameraRef.PitchDegrees);

                if (SystemAPI.HasComponent<CameraShake>(entity)) {
                    var shake = SystemAPI.GetComponent<CameraShake>(entity);
                    cameraPosition += shake.Offset;
                }

                cameraRef.Position = cameraPosition;
                cameraRef.Rotation = cameraRotation;

                break;
            }
        }

        private static float3 CalculatePosition(float3 entityPosition, quaternion entityRotation, float3 eyeOffset) {
            float3 rotatedOffset = math.rotate(entityRotation, eyeOffset);
            return entityPosition + rotatedOffset;
        }

        private static quaternion CalculateRotation(quaternion entityRotation, float pitchDegrees) {
            quaternion pitchRotation = quaternion.AxisAngle(math.right(), math.radians(pitchDegrees));
            return math.mul(entityRotation, pitchRotation);
        }
    }
}
