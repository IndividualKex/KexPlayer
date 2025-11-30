using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace KexInteract {
    public struct Interacter : IComponentData {
        public Entity Target;
        public float3 HitPosition;
        public float InteractDistance;
        public LayerMask PhysicsMask;
        public InteractionMask InteractionMask;
        [GhostField] public NetworkTick LastFireTick;
        [GhostField] public NetworkTick LastAltFireTick;
        [GhostField] public NetworkTick LastInteractTick;
        [GhostField] public NetworkTick LastAltInteractTick;
        [GhostField] public NetworkTick LastAction1Tick;
        [GhostField] public NetworkTick LastAction2Tick;
    }
}
