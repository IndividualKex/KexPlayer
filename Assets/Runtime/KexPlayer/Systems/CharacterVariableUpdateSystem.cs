using Unity.Burst;
using Unity.Entities;
using Unity.CharacterController;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;

namespace KexPlayer {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct CharacterVariableUpdateSystem : ISystem {
        private UpdateContext _context;
        private KinematicCharacterUpdateContext _baseContext;

        public void OnCreate(ref SystemState state) {
            _context = new UpdateContext();
            _context.OnSystemCreate(ref state);
            _baseContext = new KinematicCharacterUpdateContext();
            _baseContext.OnSystemCreate(ref state);

            var query = KinematicCharacterUtilities.GetBaseCharacterQueryBuilder()
                .WithAll<CharacterConfig, CharacterState, Input>()
                .Build(ref state);
            state.RequireForUpdate(query);
        }

        public void OnUpdate(ref SystemState state) {
            var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            _context.OnSystemUpdate(ref state);
            _baseContext.OnSystemUpdate(ref state, SystemAPI.Time, physicsWorldSingleton);

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
                in CharacterState state
            ) {
                ref quaternion rotationRef = ref kinematic.LocalTransform.ValueRW.Rotation;

                rotationRef = quaternion.Euler(0f, math.radians(state.BodyYawDegrees), 0f);
            }

            public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask) {
                BaseContext.EnsureCreationOfTmpCollections();
                return true;
            }

            public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted) { }
        }
    }
}
