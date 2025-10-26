using Unity.Burst;
using Unity.Entities;
using Unity.CharacterController;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.Transforms;

namespace KexCharacter {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [BurstCompile]
    public partial struct CharacterVariableUpdateSystem : ISystem {
        private EntityQuery _characterQuery;
        private UpdateContext _context;
        private KinematicCharacterUpdateContext _baseContext;

        public void OnCreate(ref SystemState state) {
            _characterQuery = KinematicCharacterUtilities.GetBaseCharacterQueryBuilder()
                .WithAll<CharacterConfig, CharacterState, CharacterInput>()
                .Build(ref state);

            _context = new UpdateContext();
            _context.OnSystemCreate(ref state);
            _baseContext = new KinematicCharacterUpdateContext();
            _baseContext.OnSystemCreate(ref state);

            state.RequireForUpdate(_characterQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            _context.OnSystemUpdate(ref state);
            _baseContext.OnSystemUpdate(ref state, SystemAPI.Time, SystemAPI.GetSingleton<Unity.Physics.PhysicsWorldSingleton>());

            new CharacterVariableUpdateJob {
                Context = _context,
                BaseContext = _baseContext,
            }.ScheduleParallel();
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct CharacterVariableUpdateJob : IJobEntity, IJobEntityChunkBeginEnd {
            public UpdateContext Context;
            public KinematicCharacterUpdateContext BaseContext;

            public void Execute(
                KinematicCharacterAspect kinematic,
                RefRW<CharacterInput> input
            ) {
                ref KinematicCharacterBody bodyRef = ref kinematic.CharacterBody.ValueRW;
                ref quaternion rotationRef = ref kinematic.LocalTransform.ValueRW.Rotation;
                ref CharacterInput inputRef = ref input.ValueRW;

                KinematicCharacterUtilities.AddVariableRateRotationFromFixedRateRotation(ref rotationRef, bodyRef.RotationFromParent, BaseContext.Time.DeltaTime, bodyRef.LastPhysicsUpdateDeltaTime);

                quaternion yawRotation = quaternion.Euler(math.up() * inputRef.YawRotationRadians);
                rotationRef = math.mul(rotationRef, yawRotation);

                inputRef.YawRotationRadians = 0f;
            }

            public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask) {
                BaseContext.EnsureCreationOfTmpCollections();
                return true;
            }

            public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted) { }
        }
    }
}
