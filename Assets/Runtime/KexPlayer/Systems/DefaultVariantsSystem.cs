using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.CharacterController;
using System.Collections.Generic;

namespace KexPlayer {
    public partial class DefaultVariantSystem : DefaultVariantSystemBase {
        protected override void RegisterDefaultVariants(Dictionary<ComponentType, Rule> defaultVariants) {
            defaultVariants.Add(typeof(KinematicCharacterBody), Rule.ForAll(typeof(KinematicCharacterBody_DefaultVariant)));
            defaultVariants.Add(typeof(CharacterInterpolation), Rule.ForAll(typeof(CharacterInterpolation_GhostVariant)));
            defaultVariants.Add(typeof(TrackedTransform), Rule.ForAll(typeof(TrackedTransform_DefaultVariant)));
        }
    }

    [GhostComponentVariation(typeof(KinematicCharacterBody))]
    [GhostComponent]
    public struct KinematicCharacterBody_DefaultVariant {
        [GhostField]
        public bool IsGrounded;
        [GhostField]
        public float3 RelativeVelocity;
    }

    [GhostComponentVariation(typeof(CharacterInterpolation))]
    [GhostComponent(PrefabType = GhostPrefabType.PredictedClient)]
    public struct CharacterInterpolation_GhostVariant { }

    [GhostComponentVariation(typeof(TrackedTransform))]
    [GhostComponent]
    public struct TrackedTransform_DefaultVariant {
        [GhostField]
        public RigidTransform CurrentFixedRateTransform;
    }
}
