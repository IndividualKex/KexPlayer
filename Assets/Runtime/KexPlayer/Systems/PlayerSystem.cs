using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using KexInput;
using KexCamera;
using KexCharacter;

namespace KexPlayer {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(CharacterVariableUpdateSystem))]
    [BurstCompile]
    public partial struct PlayerSystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            double currentTime = SystemAPI.Time.ElapsedTime;

            foreach (var (characterConfig, characterInput, camera, input) in SystemAPI
                .Query<RefRO<CharacterConfig>, RefRW<CharacterInput>, RefRW<Camera>, RefRO<Input>>()
                .WithAll<Player, Simulate>()
            ) {
                ref readonly CharacterConfig configRef = ref characterConfig.ValueRO;
                ref CharacterInput inputCharacterRef = ref characterInput.ValueRW;
                ref Camera cameraRef = ref camera.ValueRW;
                ref readonly Input inputRef = ref input.ValueRO;

                float2 lookDelta = inputRef.Look * cameraRef.LookSensitivity;

                inputCharacterRef.YawRotationRadians = math.radians(lookDelta.x);
                cameraRef.PitchDegrees += lookDelta.y;
                cameraRef.PitchDegrees = math.clamp(cameraRef.PitchDegrees, cameraRef.MinPitch, cameraRef.MaxPitch);

                inputCharacterRef.MovementInput = inputRef.Move;

                if (inputRef.Jump.WasJustPressed(currentTime, configRef.CoyoteTimeDuration)) {
                    inputCharacterRef.JumpRequestTime = currentTime;
                }
            }
        }
    }
}
