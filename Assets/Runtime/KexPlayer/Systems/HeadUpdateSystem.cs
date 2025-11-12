using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace KexPlayer {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(CharacterVariableUpdateSystem))]
    [BurstCompile]
    public partial struct HeadUpdateSystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (head, transform) in SystemAPI
                .Query<Head, RefRW<LocalTransform>>()
            ) {
                if (!SystemAPI.HasComponent<LocalToWorld>(head.Player) ||
                    !SystemAPI.HasComponent<Input>(head.Player)) continue;
                var playerTransform = SystemAPI.GetComponent<LocalTransform>(head.Player);
                var input = SystemAPI.GetComponent<Input>(head.Player);
                quaternion targetWorldRotation = CalculateRotation(input.ViewYawDegrees, input.ViewPitchDegrees);
                quaternion currentWorldRotation = playerTransform.Rotation;
                quaternion targetLocalRotation = math.mul(math.inverse(currentWorldRotation), targetWorldRotation);
                transform.ValueRW.Rotation = targetLocalRotation;
            }
        }

        private quaternion CalculateRotation(float yawDegrees, float pitchDegrees) {
            quaternion yawRotation = quaternion.Euler(0f, math.radians(yawDegrees), 0f);
            quaternion pitchRotation = quaternion.AxisAngle(math.right(), math.radians(pitchDegrees));
            return math.mul(yawRotation, pitchRotation);
        }
    }
}
