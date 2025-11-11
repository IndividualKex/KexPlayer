using Unity.Burst;
using Unity.Entities;
using Unity.CharacterController;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.NetCode;

namespace KexPlayer {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [BurstCompile]
    public partial struct CharacterVariableUpdateSystem : ISystem {
        private EntityQuery _characterQuery;
        private UpdateContext _context;
        private KinematicCharacterUpdateContext _baseContext;

        public void OnCreate(ref SystemState state) {
            _characterQuery = KinematicCharacterUtilities.GetBaseCharacterQueryBuilder()
                .WithAll<CharacterConfig, CharacterState, Input>()
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
                RefRW<Input> input
            ) {
                ref quaternion rotationRef = ref kinematic.LocalTransform.ValueRW.Rotation;
                ref readonly Input inputRef = ref input.ValueRO;

                rotationRef = quaternion.Euler(0f, math.radians(inputRef.ViewYawDegrees), 0f);
            }

            public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask) {
                BaseContext.EnsureCreationOfTmpCollections();
                return true;
            }

            public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted) { }
        }
    }
}
