using KexInteract;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(InteractSystem))]
public partial class DebugInteractionSystem : SystemBase {
    protected override void OnUpdate() {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (evt, entity) in SystemAPI
            .Query<InteractEvent>()
            .WithEntityAccess()
        ) {
            UnityEngine.Debug.Log($"Interacted with {evt.Target} by {evt.Sender} using interaction {evt.Interaction}");
            ecb.DestroyEntity(entity);
        }
        ecb.Playback(EntityManager);
    }
}
