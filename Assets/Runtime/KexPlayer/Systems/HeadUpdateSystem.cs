using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace KexPlayer {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(CharacterVariableUpdateSystem))]
    [BurstCompile]
    public partial struct HeadUpdateSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var currentTick = networkTime.ServerTick;

            state.Dependency = new HeadUpdateJob {
                InputLockTimerLookup = SystemAPI.GetComponentLookup<InputLockTimer>(true),
                CurrentTick = currentTick,
            }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        [WithAll(typeof(Player), typeof(Simulate))]
        partial struct HeadUpdateJob : IJobEntity {
            [ReadOnly] public ComponentLookup<InputLockTimer> InputLockTimerLookup;
            public NetworkTick CurrentTick;

            public void Execute(
                Entity entity,
                in Input input,
                ref HeadRotation headRotation,
                in LocalTransform playerTransform
            ) {
                bool inputLocked = InputLockTimerLookup.TryGetComponent(entity, out var timer)
                    && timer.IsLocked(CurrentTick);

                if (inputLocked) return;

                quaternion targetWorldRotation = CalculateRotation(input.ViewYawDegrees, input.ViewPitchDegrees);
                quaternion currentWorldRotation = playerTransform.Rotation;
                quaternion targetLocalRotation = math.mul(math.inverse(currentWorldRotation), targetWorldRotation);
                headRotation.LocalRotation = targetLocalRotation;
            }

            private static quaternion CalculateRotation(float yawDegrees, float pitchDegrees) {
                quaternion yawRotation = quaternion.Euler(0f, math.radians(yawDegrees), 0f);
                quaternion pitchRotation = quaternion.AxisAngle(math.right(), math.radians(pitchDegrees));
                return math.mul(yawRotation, pitchRotation);
            }
        }
    }
}
