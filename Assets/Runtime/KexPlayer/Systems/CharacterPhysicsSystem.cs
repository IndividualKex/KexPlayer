using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.CharacterController;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.NetCode;

namespace KexPlayer {
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct CharacterPhysicsSystem : ISystem {
        private UpdateContext _context;
        private KinematicCharacterUpdateContext _baseContext;

        public void OnCreate(ref SystemState state) {
            _context = new UpdateContext();
            _context.OnSystemCreate(ref state);
            _baseContext = new KinematicCharacterUpdateContext();
            _baseContext.OnSystemCreate(ref state);

            var characterQuery = KinematicCharacterUtilities.GetBaseCharacterQueryBuilder()
                .WithAll<CharacterConfig, CharacterState, Input>()
                .Build(ref state);
            state.RequireForUpdate(characterQuery);

            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            _context.OnSystemUpdate(ref state);
            _baseContext.OnSystemUpdate(ref state, SystemAPI.Time, SystemAPI.GetSingleton<PhysicsWorldSingleton>());

            new CharacterPhysicsJob {
                Context = _context,
                BaseContext = _baseContext,
            }.ScheduleParallel();
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct CharacterPhysicsJob : IJobEntity, IJobEntityChunkBeginEnd {
            public UpdateContext Context;
            public KinematicCharacterUpdateContext BaseContext;

            public void Execute(
                Entity entity,
                KinematicCharacterAspect kinematic,
                in CharacterConfig config,
                RefRW<CharacterState> state,
                RefRW<Input> input
            ) {
                var processor = new CharacterProcessor {
                    Self = entity,
                    Kinematic = kinematic,
                    Config = config,
                    State = state,
                    Input = input
                };

                processor.PhysicsUpdate(ref Context, ref BaseContext);
            }

            public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask) {
                BaseContext.EnsureCreationOfTmpCollections();
                return true;
            }

            public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted) {
            }

            private struct CharacterProcessor : IKinematicCharacterProcessor<UpdateContext> {
                public Entity Self;
                public KinematicCharacterAspect Kinematic;
                public CharacterConfig Config;
                public RefRW<CharacterState> State;
                public RefRW<Input> Input;

                public void PhysicsUpdate(ref UpdateContext context, ref KinematicCharacterUpdateContext baseContext) {
                    ref KinematicCharacterBody bodyRef = ref Kinematic.CharacterBody.ValueRW;
                    ref LocalTransform transformRef = ref Kinematic.LocalTransform.ValueRW;
                    ref float3 positionRef = ref transformRef.Position;

                    Kinematic.Update_Initialize(in this, ref context, ref baseContext, ref bodyRef, baseContext.Time.DeltaTime);
                    Kinematic.Update_ParentMovement(in this, ref context, ref baseContext, ref bodyRef, ref positionRef, bodyRef.WasGroundedBeforeCharacterUpdate);
                    Kinematic.Update_Grounding(in this, ref context, ref baseContext, ref bodyRef, ref positionRef);

                    HandleVelocityControl(ref context, ref baseContext);

                    Kinematic.Update_PreventGroundingFromFutureSlopeChange(in this, ref context, ref baseContext, ref bodyRef, in Config.StepAndSlopeHandling);
                    Kinematic.Update_GroundPushing(in this, ref context, ref baseContext, Config.Gravity);
                    Kinematic.Update_MovementAndDecollisions(in this, ref context, ref baseContext, ref bodyRef, ref positionRef);
                    Kinematic.Update_MovingPlatformDetection(ref baseContext, ref bodyRef);
                    Kinematic.Update_ParentMomentum(ref baseContext, ref bodyRef);
                    Kinematic.Update_ProcessStatefulCharacterHits();
                }

                private void HandleVelocityControl(ref UpdateContext context, ref KinematicCharacterUpdateContext baseContext) {
                    float deltaTime = baseContext.Time.DeltaTime;
                    ref KinematicCharacterBody bodyRef = ref Kinematic.CharacterBody.ValueRW;
                    ref CharacterState stateRef = ref State.ValueRW;
                    ref Input inputRef = ref Input.ValueRW;

                    quaternion cameraYawRotation = quaternion.Euler(0f, math.radians(inputRef.ViewYawDegrees), 0f);
                    float3 cameraForward = MathUtilities.GetForwardFromRotation(cameraYawRotation);
                    float3 cameraRight = MathUtilities.GetRightFromRotation(cameraYawRotation);
                    float3 moveVector = (inputRef.Move.y * cameraForward) + (inputRef.Move.x * cameraRight);
                    moveVector = MathUtilities.ClampToMaxLength(moveVector, 1f);

                    if (math.lengthsq(inputRef.Move) > 0f) {
                        stateRef.BodyYawDegrees = inputRef.ViewYawDegrees;
                    }

                    if (bodyRef.ParentEntity != Entity.Null) {
                        moveVector = math.rotate(bodyRef.RotationFromParent, moveVector);
                        bodyRef.RelativeVelocity = math.rotate(bodyRef.RotationFromParent, bodyRef.RelativeVelocity);
                    }

                    if (bodyRef.IsGrounded) {
                        stateRef.LastGroundedTime = baseContext.Time.ElapsedTime;
                    }

                    double timeSinceUngrounded = baseContext.Time.ElapsedTime - stateRef.LastGroundedTime;
                    bool isInCoyoteTime = timeSinceUngrounded <= Config.CoyoteTimeDuration;

                    if (bodyRef.IsGrounded) {
                        float3 targetVelocity = moveVector * Config.GroundMaxSpeed;
                        CharacterControlUtilities.StandardGroundMove_Interpolated(ref bodyRef.RelativeVelocity, targetVelocity, Config.GroundedMovementSharpness, deltaTime, bodyRef.GroundingUp, bodyRef.GroundHit.Normal);

                        if (inputRef.Jump.IsSet) {
                            CharacterControlUtilities.StandardJump(ref bodyRef, bodyRef.GroundingUp * Config.JumpSpeed, true, bodyRef.GroundingUp);
                        }
                    }
                    else {
                        if (inputRef.Jump.IsSet && isInCoyoteTime) {
                            CharacterControlUtilities.StandardJump(ref bodyRef, bodyRef.GroundingUp * Config.JumpSpeed, true, bodyRef.GroundingUp);
                            stateRef.LastGroundedTime = baseContext.Time.ElapsedTime - Config.CoyoteTimeDuration - 1.0;
                        }

                        float3 airAcceleration = moveVector * Config.AirAcceleration;
                        if (math.lengthsq(airAcceleration) > 0f) {
                            float3 tmpVelocity = bodyRef.RelativeVelocity;
                            CharacterControlUtilities.StandardAirMove(ref bodyRef.RelativeVelocity, airAcceleration, Config.AirMaxSpeed, bodyRef.GroundingUp, deltaTime, false);

                            if (Config.PreventAirAccelerationAgainstUngroundedHits && Kinematic.MovementWouldHitNonGroundedObstruction(in this, ref context, ref baseContext, bodyRef.RelativeVelocity * deltaTime, out ColliderCastHit hit)) {
                                bodyRef.RelativeVelocity = tmpVelocity;
                            }
                        }

                        CharacterControlUtilities.AccelerateVelocity(ref bodyRef.RelativeVelocity, Config.Gravity, deltaTime);
                        CharacterControlUtilities.ApplyDragToVelocity(ref bodyRef.RelativeVelocity, deltaTime, Config.AirDrag);
                    }
                }

                public void UpdateGroundingUp(ref UpdateContext context, ref KinematicCharacterUpdateContext baseContext) {
                    ref KinematicCharacterBody bodyRef = ref Kinematic.CharacterBody.ValueRW;
                    Kinematic.Default_UpdateGroundingUp(ref bodyRef);
                }

                public bool CanCollideWithHit(ref UpdateContext context, ref KinematicCharacterUpdateContext baseContext, in BasicHit hit) {
                    return PhysicsUtilities.IsCollidable(hit.Material);
                }

                public bool IsGroundedOnHit(ref UpdateContext context, ref KinematicCharacterUpdateContext baseContext, in BasicHit hit, int groundingEvaluationType) {
                    return Kinematic.Default_IsGroundedOnHit(in this, ref context, ref baseContext, in hit, in Config.StepAndSlopeHandling, groundingEvaluationType);
                }

                public void OnMovementHit(ref UpdateContext context, ref KinematicCharacterUpdateContext baseContext, ref KinematicCharacterHit hit, ref float3 remainingMovementDirection, ref float remainingMovementLength, float3 originalVelocityDirection, float hitDistance) {
                    ref KinematicCharacterBody bodyRef = ref Kinematic.CharacterBody.ValueRW;
                    ref LocalTransform transformRef = ref Kinematic.LocalTransform.ValueRW;
                    ref float3 positionRef = ref transformRef.Position;

                    Kinematic.Default_OnMovementHit(
                        in this,
                        ref context,
                        ref baseContext,
                        ref bodyRef,
                        ref positionRef,
                        ref hit,
                        ref remainingMovementDirection,
                        ref remainingMovementLength,
                        originalVelocityDirection,
                        hitDistance,
                        Config.StepAndSlopeHandling.StepHandling,
                        Config.StepAndSlopeHandling.MaxStepHeight,
                        Config.StepAndSlopeHandling.CharacterWidthForStepGroundingCheck);
                }

                public void OverrideDynamicHitMasses(ref UpdateContext context, ref KinematicCharacterUpdateContext baseContext, ref PhysicsMass characterMass, ref PhysicsMass otherMass, BasicHit hit) {
                }

                public void ProjectVelocityOnHits(ref UpdateContext context, ref KinematicCharacterUpdateContext baseContext, ref float3 velocity, ref bool characterIsGrounded, ref BasicHit characterGroundHit, in DynamicBuffer<KinematicVelocityProjectionHit> velocityProjectionHits, float3 originalVelocityDirection) {
                    Kinematic.Default_ProjectVelocityOnHits(
                        ref velocity,
                        ref characterIsGrounded,
                        ref characterGroundHit,
                        in velocityProjectionHits,
                        originalVelocityDirection,
                        Config.StepAndSlopeHandling.ConstrainVelocityToGroundPlane);
                }
            }
        }
    }

    public struct UpdateContext {
        public void OnSystemCreate(ref SystemState state) {
        }

        public void OnSystemUpdate(ref SystemState state) {
        }
    }
}
