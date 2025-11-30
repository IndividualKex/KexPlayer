using KexPlayer;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct InputLockTimerSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (timer, capabilities) in SystemAPI
            .Query<RefRW<InputLockTimer>, RefRW<PlayerCapabilities>>()
            .WithAll<Simulate>()
        ) {
            if (timer.ValueRO.RemainingTime > 0) {
                timer.ValueRW.RemainingTime -= dt;
                capabilities.ValueRW.Flags = CapabilityFlags.None;
            }
            else {
                capabilities.ValueRW.Flags = CapabilityFlags.All;
            }
        }
    }
}
