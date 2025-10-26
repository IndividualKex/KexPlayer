using Unity.Entities;
using Unity.Mathematics;
using Unity.CharacterController;

namespace KexCharacter {
    public struct CharacterConfig : IComponentData {
        public BasicStepAndSlopeHandlingParameters StepAndSlopeHandling;
        public float3 Gravity;
        public float GroundMaxSpeed;
        public float GroundedMovementSharpness;
        public float AirAcceleration;
        public float AirMaxSpeed;
        public float AirDrag;
        public float JumpSpeed;
        public float CoyoteTimeDuration;
        public bool PreventAirAccelerationAgainstUngroundedHits;
    }
}
