using KexInteract;
using KexPlayer;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[UpdateAfter(typeof(InteractSystem))]
[BurstCompile]
public partial struct DebugInteractionSystem : ISystem {
    private const uint LockDurationTicks = 12;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var networkTime = SystemAPI.GetSingleton<NetworkTime>();
        if (!networkTime.IsFirstTimeFullyPredictingTick) return;

        var currentTick = networkTime.ServerTick;
        var inputLockTimerLookup = SystemAPI.GetComponentLookup<InputLockTimer>();
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (evt, entity) in SystemAPI.Query<InteractEvent>().WithEntityAccess()) {
            if (inputLockTimerLookup.TryGetRefRW(evt.Sender, out var timer)) {
                var unlockTick = currentTick;
                unlockTick.Add(LockDurationTicks);
                timer.ValueRW.UnlockTick = unlockTick;
            }
            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
    }
}
