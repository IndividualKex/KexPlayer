using Unity.Burst;
using Unity.Entities;
using Unity.Collections;

namespace KexInteract {
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct InteractEventCleanupSystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (evt, entity) in SystemAPI
                .Query<InteractEvent>()
                .WithEntityAccess()
            ) {
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
