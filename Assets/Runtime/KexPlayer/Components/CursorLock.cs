using Unity.Entities;

namespace KexPlayer {
    public struct CursorLock : IComponentData {
        public bool Value;

        public static implicit operator bool(CursorLock cursorLock) => cursorLock.Value;
        public static implicit operator CursorLock(bool value) => new() { Value = value };
    }
}
