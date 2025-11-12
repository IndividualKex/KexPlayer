using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using KexPlayer;

namespace KexInteract {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PredictedSimulationSystemGroup))]
    [BurstCompile]
    public partial struct InteractSystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (interacter, input, entity) in SystemAPI
                .Query<Interacter, Input>()
                .WithAll<GhostOwnerIsLocal>()
                .WithEntityAccess()
            ) {
                if (interacter.Target == Entity.Null) continue;

                var interactable = SystemAPI.GetComponent<Interactable>(interacter.Target);

                if (input.Fire.IsSet && (interactable.ControlMask & ControlMask.Fire) == ControlMask.Fire) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 0);
                }
                if (input.AltFire.IsSet && (interactable.ControlMask & ControlMask.AltFire) == ControlMask.AltFire) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 1);
                }
                if (input.Interact.IsSet && (interactable.ControlMask & ControlMask.Interact) == ControlMask.Interact) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 2);
                }
                if (input.AltInteract.IsSet && (interactable.ControlMask & ControlMask.AltInteract) == ControlMask.AltInteract) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 3);
                }
                if (input.Action1.IsSet && (interactable.ControlMask & ControlMask.Action1) == ControlMask.Action1) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 4);
                }
                if (input.Action2.IsSet && (interactable.ControlMask & ControlMask.Action2) == ControlMask.Action2) {
                    CreateInteractEvent(ecb, interacter.Target, entity, 5);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private Entity CreateInteractEvent(EntityCommandBuffer ecb, Entity target, Entity sender, byte interaction) {
            Entity eventEntity = ecb.CreateEntity();
            ecb.AddComponent(eventEntity, new InteractEvent {
                Target = target,
                Sender = sender,
                Interaction = interaction
            });
            return eventEntity;
        }
    }
}
