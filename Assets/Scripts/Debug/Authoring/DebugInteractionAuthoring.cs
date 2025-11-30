using Unity.Entities;
using UnityEngine;

public class DebugInteractionAuthoring : MonoBehaviour {
    public class Baker : Baker<DebugInteractionAuthoring> {
        public override void Bake(DebugInteractionAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<DebugInteraction>(entity);
        }
    }
}
