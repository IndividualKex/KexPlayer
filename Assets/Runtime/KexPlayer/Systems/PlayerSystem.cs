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

            foreach (var (config, characterInput, camera, input) in SystemAPI
                .Query<CharacterConfig, RefRW<CharacterInput>, RefRW<Camera>, RefRW<Input>>()
                .WithAll<Player, Simulate>()
            ) {
                ref CharacterInput inputCharacterRef = ref characterInput.ValueRW;
                ref Camera cameraRef = ref camera.ValueRW;
                ref Input inputRef = ref input.ValueRW;

                float2 lookDelta = inputRef.Look * cameraRef.LookSensitivity;

                inputCharacterRef.YawRotationRadians = math.radians(lookDelta.x);
                cameraRef.PitchDegrees += lookDelta.y;
                cameraRef.PitchDegrees = math.clamp(cameraRef.PitchDegrees, cameraRef.MinPitch, cameraRef.MaxPitch);

                inputCharacterRef.MovementInput = inputRef.Move;

                if (inputRef.Jump.WasJustPressed(currentTime, config.CoyoteTimeDuration)) {
                    inputCharacterRef.JumpRequestTime = currentTime;
                    inputRef.Jump.Consume();
                }
            }
        }
    }
}
