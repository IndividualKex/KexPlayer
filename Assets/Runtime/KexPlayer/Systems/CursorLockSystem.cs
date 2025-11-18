using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KexPlayer {
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [UpdateBefore(typeof(InputSystem))]
    public partial class CursorLockSystem : SystemBase {
        protected override void OnUpdate() {
            foreach (var cursorLock in SystemAPI.Query<RefRW<CursorLock>>().WithAll<GhostOwnerIsLocal>()) {
                if (Keyboard.current.escapeKey.wasPressedThisFrame) {
                    cursorLock.ValueRW.Value = false;
                }

                if (!cursorLock.ValueRO.Value && Mouse.current.leftButton.wasPressedThisFrame) {
                    cursorLock.ValueRW.Value = true;
                }

                if (cursorLock.ValueRO.Value) {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                } else {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
    }
}
