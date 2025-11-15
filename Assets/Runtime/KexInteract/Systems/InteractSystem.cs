using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Mathematics;
using KexPlayer;

namespace KexInteract {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [BurstCompile]
    public partial struct InteractSystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!networkTime.IsFirstTimeFullyPredictingTick) return;

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (interacter, input, entity) in SystemAPI
                .Query<Interacter, Input>()
                .WithAll<GhostOwnerIsLocal, Simulate>()
                .WithEntityAccess()
            ) {
                if (interacter.Target == Entity.Null) continue;

                var interactable = SystemAPI.GetComponent<Interactable>(interacter.Target);

                if (input.Fire.IsSet && (interactable.ControlMask & ControlMask.Fire) == ControlMask.Fire) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 0, interacter.HitPosition);
                }
                if (input.AltFire.IsSet && (interactable.ControlMask & ControlMask.AltFire) == ControlMask.AltFire) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 1, interacter.HitPosition);
                }
                if (input.Interact.IsSet && (interactable.ControlMask & ControlMask.Interact) == ControlMask.Interact) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 2, interacter.HitPosition);
                }
                if (input.AltInteract.IsSet && (interactable.ControlMask & ControlMask.AltInteract) == ControlMask.AltInteract) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 3, interacter.HitPosition);
                }
                if (input.Action1.IsSet && (interactable.ControlMask & ControlMask.Action1) == ControlMask.Action1) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 4, interacter.HitPosition);
                }
                if (input.Action2.IsSet && (interactable.ControlMask & ControlMask.Action2) == ControlMask.Action2) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 5, interacter.HitPosition);
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
    }
}
