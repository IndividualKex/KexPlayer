using Unity.Entities;

namespace KexOutline {
    public struct OutlineConfig : IComponentData {
        public int OutlineLayer;
        public int DefaultLayer;
    }
}
