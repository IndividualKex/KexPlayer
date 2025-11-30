using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace KexInteract {
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct InteractEventCleanupSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state) {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var currentTick = networkTime.ServerTick;
            if (!currentTick.IsValid) return;

            var staleTick = currentTick;
            staleTick.Subtract(2);

            int staleCount = 0;

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (evt, entity) in SystemAPI
                .Query<InteractEvent>()
                .WithEntityAccess()
            ) {
                if (!evt.Tick.IsValid || staleTick.IsNewerThan(evt.Tick)) {
                    staleCount++;
                    ecb.DestroyEntity(entity);
                }
            }
            ecb.Playback(state.EntityManager);

            if (staleCount > 0) {
                string source = state.World.IsClient() ? "Client" : "Server";
                UnityEngine.Debug.LogWarning($"[InteractEventCleanupSystem] {source}: {staleCount} InteractEvent(s) were not consumed by any system");
            }
        }
    }
}
