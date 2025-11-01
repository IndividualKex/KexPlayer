using Unity.Entities;
using UnityEngine;

namespace KexOutline {
    public class OutlineRendererAuthoring : MonoBehaviour {
        public GameObject Renderer;

        private class Baker : Baker<OutlineRendererAuthoring> {
            public override void Bake(OutlineRendererAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                Entity renderer = Entity.Null;
                int rendererLayer = 0;
                if (authoring.Renderer != null) {
                    renderer = GetEntity(authoring.Renderer, TransformUsageFlags.Dynamic);
                    rendererLayer = authoring.Renderer.layer;
                }

                AddComponent(entity, new OutlineRenderer {
                    Renderer = renderer,
                    RendererLayer = rendererLayer
                });
            }
        }
    }
}
