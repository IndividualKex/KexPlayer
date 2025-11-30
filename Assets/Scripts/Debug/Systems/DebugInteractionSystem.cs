using KexInteract;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct DebugInteractionSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var inputLockTimerLookup = SystemAPI.GetComponentLookup<InputLockTimer>();
        using var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (evt, entity) in SystemAPI.Query<InteractEvent>().WithEntityAccess()) {
            if (inputLockTimerLookup.TryGetRefRW(evt.Sender, out var timer)) {
                timer.ValueRW.RemainingTime = 0.2f;
            }
            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
    }
}
