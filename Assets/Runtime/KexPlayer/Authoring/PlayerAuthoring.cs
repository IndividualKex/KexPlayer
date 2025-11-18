using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.CharacterController;

namespace KexPlayer {
    public class PlayerAuthoring : MonoBehaviour {
        [Header("Character Physics")]
        public AuthoringKinematicCharacterProperties CharacterProperties = AuthoringKinematicCharacterProperties.GetDefault();
        public BasicStepAndSlopeHandlingParameters StepAndSlopeHandling = BasicStepAndSlopeHandlingParameters.GetDefault();
        public float3 Gravity = math.up() * -28f;

        [Header("Movement")]
        public float GroundMaxSpeed = 5f;
        public float GroundedMovementSharpness = 30f;
        public float AirAcceleration = 50f;
        public float AirMaxSpeed = 5f;
        public float AirDrag = 0f;
        public float JumpSpeed = 7.5f;
        public float CoyoteTimeDuration = 0.15f;
        public bool PreventAirAccelerationAgainstUngroundedHits = true;

        [Header("Camera")]
        public float3 EyeOffset = new(0f, 1.6f, 0f);
        public float MinPitch = -89f;
        public float MaxPitch = 89f;
        public float LookInputSensitivity = 0.1f;

        public class Baker : Baker<PlayerAuthoring> {
            public override void Bake(PlayerAuthoring authoring) {
                KinematicCharacterUtilities.BakeCharacter(this, authoring.gameObject, authoring.CharacterProperties);

                Entity entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.WorldSpace);

                AddComponent(entity, new CharacterConfig {
                    StepAndSlopeHandling = authoring.StepAndSlopeHandling,
                    Gravity = authoring.Gravity,
                    GroundMaxSpeed = authoring.GroundMaxSpeed,
                    GroundedMovementSharpness = authoring.GroundedMovementSharpness,
                    AirAcceleration = authoring.AirAcceleration,
                    AirMaxSpeed = authoring.AirMaxSpeed,
                    AirDrag = authoring.AirDrag,
                    JumpSpeed = authoring.JumpSpeed,
                    CoyoteTimeDuration = authoring.CoyoteTimeDuration,
                    PreventAirAccelerationAgainstUngroundedHits = authoring.PreventAirAccelerationAgainstUngroundedHits,
                });
                float initialYaw = authoring.transform.eulerAngles.y;
                AddComponent(entity, new CharacterState {
                    LastGroundedTime = 0,
                    BodyYawDegrees = initialYaw,
                });
                AddComponent<Player>(entity);
                AddComponent<Input>(entity);
                AddComponent(entity, new Camera {
                    YawDegrees = 0f,
                    PitchDegrees = 0f,
                    MinPitch = authoring.MinPitch,
                    MaxPitch = authoring.MaxPitch,
                    LookSensitivity = authoring.LookInputSensitivity,
                    EyeOffset = authoring.EyeOffset,
                });
                AddComponent(entity, new HeadRotation {
                    LocalRotation = quaternion.identity,
                });
                AddComponent(entity, new CursorLock {
                    Value = true,
                });
            }
        }
    }
}
