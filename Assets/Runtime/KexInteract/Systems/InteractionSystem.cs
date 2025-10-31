using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Unity.CharacterController;
using KexInput;
using KexCamera;
using KexCharacter;

namespace KexInteract {
    [UpdateInGroup(typeof(KinematicCharacterPhysicsUpdateGroup))]
    [UpdateAfter(typeof(CharacterPhysicsSystem))]
    [BurstCompile]
    public partial struct InteractionSystem : ISystem {
        private const float MaxDistance = 3f;
        private const float DistanceFactor = 2f;
        private const float AngleFactor = 0.5f;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            var hits = new NativeList<RaycastHit>(Allocator.Temp);

            double elapsedTime = SystemAPI.Time.ElapsedTime;

            foreach (var (interacter, input, camera, entity) in SystemAPI.Query<RefRW<Interacter>, RefRW<Input>, Camera>().WithEntityAccess()) {
                ref var interacterRef = ref interacter.ValueRW;
                ref var inputRef = ref input.ValueRW;

                var ray = new RaycastInput {
                    Start = camera.Position,
                    End = camera.Position + math.forward(camera.Rotation) * MaxDistance,
                    Filter = new CollisionFilter {
                        BelongsTo = ~0u,
                        CollidesWith = interacterRef.LayerMask,
                    },
                };

                Entity interactableEntity = Entity.Null;
                Interactable interactable = default;
                hits.Clear();
                if (collisionWorld.CastRay(ray, ref hits)) {
                    float bestScore = 0f;
                    float3 rayDirection = math.forward(camera.Rotation);

                    for (int i = 0; i < hits.Length; i++) {
                        var hit = hits[i];
                        if (SystemAPI.HasComponent<InteractionBlocker>(hit.Entity)) continue;
                        if (!SystemAPI.HasComponent<Interactable>(hit.Entity)) continue;

                        var candidate = SystemAPI.GetComponent<Interactable>(hit.Entity);
                        if (!candidate.AllowInteract) continue;
                        if (candidate.Mask != 0 && interacterRef.Mask != 0 && (candidate.Mask & interacterRef.Mask) == 0) continue;

                        var transform = SystemAPI.GetComponent<LocalToWorld>(hit.Entity);
                        float score = CalculateInteractionScore(transform.Position, camera.Position, rayDirection);

                        if (score > bestScore) {
                            bestScore = score;
                            interactableEntity = hit.Entity;
                            interactable = candidate;
                        }
                    }
                }

                if (interactableEntity != interacterRef.Target) {
                    interacterRef.Target = interactableEntity;

                    if (inputRef.Action1.IsPressed()) {
                        inputRef.Action1.Consume();
                        inputRef.Action1.Press(elapsedTime);
                    }
                }

                if (inputRef.Action1.WasClicked(elapsedTime) &&
                    interactableEntity != Entity.Null &&
                    interactable.CanInteract
                ) {
                    inputRef.Action1.Consume();
                    CreateInteractEvent(ecb, interactableEntity, entity, 0, InteractInputType.Click);
                }

                if (inputRef.Action2.WasClicked(elapsedTime) &&
                    interactableEntity != Entity.Null &&
                    interactable.CanAltInteract
                ) {
                    inputRef.Action2.Consume();
                    CreateInteractEvent(ecb, interactableEntity, entity, 1, InteractInputType.Click);
                }

                if (inputRef.Action3.WasClicked(elapsedTime) &&
                    interactableEntity != Entity.Null &&
                    interactable.CanPush
                ) {
                    inputRef.Action3.Consume();
                    CreateInteractEvent(ecb, interactableEntity, entity, 2, InteractInputType.Click);
                }

                if (interactableEntity != Entity.Null &&
                    interactable.CanHoldInteract &&
                    inputRef.Action1.IsHeldFor(elapsedTime)
                ) {
                    inputRef.Action1.Consume();
                    CreateInteractEvent(ecb, interactableEntity, entity, 0, InteractInputType.Hold);
                }
            }

            hits.Dispose();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private float CalculateInteractionScore(float3 hitPosition, float3 rayOrigin, float3 rayDirection) {
            float3 hitVector = hitPosition - rayOrigin;
            float distance = math.length(hitVector);
            float3 hitDirection = math.normalize(hitVector);

            float distanceWeight = 1.0f / (1.0f + distance * DistanceFactor);
            float angleWeight = math.max(0f, math.dot(rayDirection, hitDirection));

            return distanceWeight * angleWeight * AngleFactor;
        }

        private Entity CreateInteractEvent(EntityCommandBuffer ecb, Entity target, Entity sender, byte buttonIndex, InteractInputType inputType) {
            Entity eventEntity = ecb.CreateEntity();
            ecb.AddComponent(eventEntity, new InteractEvent {
                Target = target,
                Sender = sender,
                ButtonIndex = buttonIndex,
                InputType = inputType
            });
            return eventEntity;
        }
    }
}
