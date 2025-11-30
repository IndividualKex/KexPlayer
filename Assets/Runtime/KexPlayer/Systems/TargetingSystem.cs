using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace KexPlayer {
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    [BurstCompile]
    public partial struct TargetingSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true);

            foreach (var (target, config, camera) in SystemAPI
                .Query<RefRW<Target>, TargetingConfig, Camera>()
            ) {
                if (camera.HeadEntity == Entity.Null) continue;
                if (!localToWorldLookup.HasComponent(camera.HeadEntity)) continue;

                var headTransform = localToWorldLookup[camera.HeadEntity];
                float3 position = headTransform.Position;
                float3 direction = headTransform.Forward;

                var ray = new RaycastInput {
                    Start = position,
                    End = position + direction * config.Distance,
                    Filter = new CollisionFilter {
                        BelongsTo = ~0u,
                        CollidesWith = config.PhysicsLayerMask,
                    },
                };

                if (collisionWorld.CastRay(ray, out var hit)) {
                    target.ValueRW.Value = hit.Entity;
                    target.ValueRW.HitPosition = hit.Position;
                }
                else {
                    target.ValueRW.Value = Entity.Null;
                    target.ValueRW.HitPosition = float3.zero;
                }
            }
        }
    }
}
