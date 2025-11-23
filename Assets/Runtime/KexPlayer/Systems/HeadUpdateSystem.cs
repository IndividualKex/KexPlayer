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
            foreach (var (input, headRotation, playerTransform) in SystemAPI
                .Query<Input, RefRW<HeadRotation>, LocalTransform>()
                .WithAll<Player, GhostOwnerIsLocal>()
            ) {
                quaternion targetWorldRotation = CalculateRotation(input.ViewYawDegrees, input.ViewPitchDegrees);
                quaternion currentWorldRotation = playerTransform.Rotation;
                quaternion targetLocalRotation = math.mul(math.inverse(currentWorldRotation), targetWorldRotation);
                headRotation.ValueRW.LocalRotation = targetLocalRotation;
            }
        }

        private static quaternion CalculateRotation(float yawDegrees, float pitchDegrees) {
            quaternion yawRotation = quaternion.Euler(0f, math.radians(yawDegrees), 0f);
            quaternion pitchRotation = quaternion.AxisAngle(math.right(), math.radians(pitchDegrees));
            return math.mul(yawRotation, pitchRotation);
        }
    }
}
