using KexPlayer;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

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

            var currentTick = networkTime.ServerTick;

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (interacter, input, entity) in SystemAPI
                .Query<RefRW<Interacter>, Input>()
                .WithAll<Simulate>()
                .WithNone<InteractionBlocker>()
                .WithEntityAccess()
            ) {
                if (interacter.ValueRO.Target == Entity.Null) continue;

                var interactable = SystemAPI.GetComponent<Interactable>(interacter.ValueRO.Target);

                if (input.Fire.IsSet && (interactable.ControlMask & ControlMask.Fire) == ControlMask.Fire) {
                    if (currentTick != interacter.ValueRO.LastFireTick) {
                        CreateInteractEvent(ecb, interacter.ValueRO.Target, entity, 0, interacter.ValueRO.HitPosition, currentTick);
                        interacter.ValueRW.LastFireTick = currentTick;
                    }
                }
                if (input.AltFire.IsSet && (interactable.ControlMask & ControlMask.AltFire) == ControlMask.AltFire) {
                    if (currentTick != interacter.ValueRO.LastAltFireTick) {
                        CreateInteractEvent(ecb, interacter.ValueRO.Target, entity, 1, interacter.ValueRO.HitPosition, currentTick);
                        interacter.ValueRW.LastAltFireTick = currentTick;
                    }
                }
                if (input.Interact.IsSet && (interactable.ControlMask & ControlMask.Interact) == ControlMask.Interact) {
                    if (currentTick != interacter.ValueRO.LastInteractTick) {
                        CreateInteractEvent(ecb, interacter.ValueRO.Target, entity, 2, interacter.ValueRO.HitPosition, currentTick);
                        interacter.ValueRW.LastInteractTick = currentTick;
                    }
                }
                if (input.AltInteract.IsSet && (interactable.ControlMask & ControlMask.AltInteract) == ControlMask.AltInteract) {
                    if (currentTick != interacter.ValueRO.LastAltInteractTick) {
                        CreateInteractEvent(ecb, interacter.ValueRO.Target, entity, 3, interacter.ValueRO.HitPosition, currentTick);
                        interacter.ValueRW.LastAltInteractTick = currentTick;
                    }
                }
                if (input.Action1.IsSet && (interactable.ControlMask & ControlMask.Action1) == ControlMask.Action1) {
                    if (currentTick != interacter.ValueRO.LastAction1Tick) {
                        CreateInteractEvent(ecb, interacter.ValueRO.Target, entity, 4, interacter.ValueRO.HitPosition, currentTick);
                        interacter.ValueRW.LastAction1Tick = currentTick;
                    }
                }
                if (input.Action2.IsSet && (interactable.ControlMask & ControlMask.Action2) == ControlMask.Action2) {
                    if (currentTick != interacter.ValueRO.LastAction2Tick) {
                        CreateInteractEvent(ecb, interacter.ValueRO.Target, entity, 5, interacter.ValueRO.HitPosition, currentTick);
                        interacter.ValueRW.LastAction2Tick = currentTick;
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }

        private Entity CreateInteractEvent(
            EntityCommandBuffer ecb,
            Entity target,
            Entity sender,
            byte interaction,
            float3 hitPosition,
            NetworkTick tick
        ) {
            Entity eventEntity = ecb.CreateEntity();
            ecb.AddComponent(eventEntity, new InteractEvent {
                Target = target,
                Sender = sender,
                Interaction = interaction,
                HitPosition = hitPosition,
                Tick = tick,
            });
            return eventEntity;
        }
    }
}
