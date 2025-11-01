using Unity.Entities;

namespace KexOutline {
    public struct LinkedOutline : IComponentData {
        public Entity Value;

        public static implicit operator Entity(LinkedOutline linkedOutline) => linkedOutline.Value;
        public static implicit operator LinkedOutline(Entity entity) => new() { Value = entity };
    }
}
