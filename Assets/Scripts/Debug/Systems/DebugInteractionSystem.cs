using KexPlayer;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct DebugInteractionSystem : ISystem {
    private const uint LockDurationTicks = 30;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
    }

    public void OnUpdate(ref SystemState state) {
        var networkTime = SystemAPI.GetSingleton<NetworkTime>();
        var currentTick = networkTime.ServerTick;

        foreach (var (input, inputLockTimer) in SystemAPI
            .Query<Input, RefRW<InputLockTimer>>()
            .WithAll<Simulate>()
        ) {
            if (!input.Fire.IsSet) continue;

            inputLockTimer.ValueRW.LockTick = currentTick;
            inputLockTimer.ValueRW.LockDurationTicks = LockDurationTicks;
        }

        if (!networkTime.IsFirstTimeFullyPredictingTick) return;

        foreach (var (interaction, input, target) in SystemAPI
            .Query<RefRW<DebugInteraction>, Input, Target>()
            .WithAll<Simulate>()
        ) {
            if (!input.Fire.IsSet) continue;
            if (target.Value == Entity.Null) continue;

            interaction.ValueRW.Tick = currentTick;
        }
    }
}
