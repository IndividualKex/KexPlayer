using Unity.Entities;

namespace KexOutline {
    public struct OutlineRenderer : IComponentData {
        public Entity Renderer;
        public int RendererLayer;
    }
}
