using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;
using KexPlayer;

namespace KexInteract {
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(CharacterPhysicsSystem))]
    [BurstCompile]
    public partial struct InteractTargetingSystem : ISystem {
        private const float DistanceFactor = 2f;
        private const float AngleFactor = 0.5f;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true);
            var hits = new NativeList<RaycastHit>(Allocator.Temp);

            foreach (var (interacter, headRotation, camera) in SystemAPI
                .Query<RefRW<Interacter>, HeadRotation, Camera>()
            ) {
                if (camera.HeadEntity == Entity.Null) continue;
                if (!localToWorldLookup.HasComponent(camera.HeadEntity)) continue;

                var headTransform = localToWorldLookup[camera.HeadEntity];
                float3 position = headTransform.Position;
                quaternion rotation = headRotation.LocalRotation;

                var ray = new RaycastInput {
                    Start = position,
                    End = position + math.forward(rotation) * interacter.ValueRO.InteractDistance,
                    Filter = new CollisionFilter {
                        BelongsTo = ~0u,
                        CollidesWith = (uint)interacter.ValueRO.PhysicsMask.value,
                    },
                };

                Entity interactableEntity = Entity.Null;
                float3 hitPosition = float3.zero;
                hits.Clear();
                if (collisionWorld.CastRay(ray, ref hits)) {
                    float bestScore = 0f;
                    float3 rayDirection = math.forward(rotation);

                    for (int i = 0; i < hits.Length; i++) {
                        var hit = hits[i];
                        if (SystemAPI.HasComponent<InteractionBlocker>(hit.Entity)) continue;
                        if (!SystemAPI.HasComponent<Interactable>(hit.Entity)) continue;

                        var candidate = SystemAPI.GetComponent<Interactable>(hit.Entity);
                        if ((candidate.InteractionMask & interacter.ValueRO.InteractionMask) == 0) continue;

                        float score = CalculateInteractionScore(hit.Position, position, rayDirection);

                        if (score > bestScore) {
                            bestScore = score;
                            interactableEntity = hit.Entity;
                            hitPosition = hit.Position;
                        }
                    }
                }

                interacter.ValueRW.Target = interactableEntity;
                interacter.ValueRW.HitPosition = hitPosition;
            }

            hits.Dispose();
        }

        private float CalculateInteractionScore(float3 hitPosition, float3 rayOrigin, float3 rayDirection) {
            float3 hitVector = hitPosition - rayOrigin;
            float distance = math.length(hitVector);
            float3 hitDirection = math.normalize(hitVector);

            float distanceWeight = 1.0f / (1.0f + distance * DistanceFactor);
            float angleWeight = math.max(0f, math.dot(rayDirection, hitDirection));

            return distanceWeight * angleWeight * AngleFactor;
        }
    }
}
