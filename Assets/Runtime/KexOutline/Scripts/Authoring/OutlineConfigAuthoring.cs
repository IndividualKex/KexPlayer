using Unity.Entities;
using UnityEngine;

namespace KexOutline {
    public class OutlineConfigAuthoring : MonoBehaviour {
        public int OutlineLayer = 8;
        public int DefaultLayer = 0;

        private class Baker : Baker<OutlineConfigAuthoring> {
            public override void Bake(OutlineConfigAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new OutlineConfig {
                    OutlineLayer = authoring.OutlineLayer,
                    DefaultLayer = authoring.DefaultLayer
                });
            }
        }
    }
}
