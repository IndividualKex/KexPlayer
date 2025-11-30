using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using KexPlayer;

namespace KexInteract {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [BurstCompile]
    public partial struct InteractSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!networkTime.IsFirstTimeFullyPredictingTick) return;

            var inputBufferLookup = SystemAPI.GetComponentLookup<InputBuffer>(false);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (interacter, input, entity) in SystemAPI
                .Query<Interacter, Input>()
                .WithAll<GhostOwnerIsLocal, Simulate>()
                .WithNone<InteractionBlocker>()
                .WithEntityAccess()
            ) {
                if (interacter.Target == Entity.Null) continue;

                var interactable = SystemAPI.GetComponent<Interactable>(interacter.Target);

                if (input.Fire.IsSet && (interactable.ControlMask & ControlMask.Fire) == ControlMask.Fire) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 0, interacter.HitPosition);
                    ClearInputBuffer(ref inputBufferLookup, entity, InputBufferField.Fire);
                }
                if (input.AltFire.IsSet && (interactable.ControlMask & ControlMask.AltFire) == ControlMask.AltFire) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 1, interacter.HitPosition);
                    ClearInputBuffer(ref inputBufferLookup, entity, InputBufferField.AltFire);
                }
                if (input.Interact.IsSet && (interactable.ControlMask & ControlMask.Interact) == ControlMask.Interact) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 2, interacter.HitPosition);
                    ClearInputBuffer(ref inputBufferLookup, entity, InputBufferField.Interact);
                }
                if (input.AltInteract.IsSet && (interactable.ControlMask & ControlMask.AltInteract) == ControlMask.AltInteract) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 3, interacter.HitPosition);
                    ClearInputBuffer(ref inputBufferLookup, entity, InputBufferField.AltInteract);
                }
                if (input.Action1.IsSet && (interactable.ControlMask & ControlMask.Action1) == ControlMask.Action1) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 4, interacter.HitPosition);
                    ClearInputBuffer(ref inputBufferLookup, entity, InputBufferField.Action1);
                }
                if (input.Action2.IsSet && (interactable.ControlMask & ControlMask.Action2) == ControlMask.Action2) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 5, interacter.HitPosition);
                    ClearInputBuffer(ref inputBufferLookup, entity, InputBufferField.Action2);
                }
            }
            ecb.Playback(state.EntityManager);
        }

        private Entity CreateInteractEvent(EntityCommandBuffer ecb, Entity target, Entity sender, byte interaction, float3 hitPosition) {
            Entity eventEntity = ecb.CreateEntity();
            ecb.AddComponent(eventEntity, new InteractEvent {
                Target = target,
                Sender = sender,
                Interaction = interaction,
                HitPosition = hitPosition
            });
            return eventEntity;
        }

        private static void ClearInputBuffer(ref ComponentLookup<InputBuffer> lookup, Entity entity, InputBufferField field) {
            if (!lookup.HasComponent(entity)) return;
            var buffer = lookup[entity];
            buffer.Clear(field);
            lookup[entity] = buffer;
        }
    }
}
