using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KexPlayer {
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [AlwaysSynchronizeSystem]
    public partial class InputSystem : SystemBase {
        protected override void OnUpdate() {
            foreach (var (input, camera, cursorLock) in SystemAPI
                .Query<RefRW<Input>, RefRW<Camera>, RefRW<CursorLock>>()
                .WithAll<GhostOwnerIsLocal>()
            ) {
                bool wasLocked = cursorLock.ValueRO.Value;

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

                bool justLocked = !wasLocked && cursorLock.ValueRO.Value;

                if (!cursorLock.ValueRO.Value) {
                    input.ValueRW = default;
                    input.ValueRW.ViewYawDegrees = camera.ValueRO.YawDegrees;
                    input.ValueRW.ViewPitchDegrees = camera.ValueRO.PitchDegrees;
                    continue;
                }

                input.ValueRW.Jump = default;
                if (Keyboard.current.spaceKey.wasPressedThisFrame) input.ValueRW.Jump.Set();

                input.ValueRW.Crouch = default;
                if (Keyboard.current.leftCtrlKey.wasPressedThisFrame) input.ValueRW.Crouch.Set();

                input.ValueRW.Sprint = default;
                if (Keyboard.current.leftShiftKey.wasPressedThisFrame) input.ValueRW.Sprint.Set();

                input.ValueRW.Fire = default;
                if (!justLocked && Mouse.current.leftButton.wasPressedThisFrame) input.ValueRW.Fire.Set();

                input.ValueRW.AltFire = default;
                if (Mouse.current.rightButton.wasPressedThisFrame) input.ValueRW.AltFire.Set();

                input.ValueRW.Interact = default;
                if (Keyboard.current.eKey.wasPressedThisFrame) input.ValueRW.Interact.Set();

                input.ValueRW.AltInteract = default;
                if (Keyboard.current.fKey.wasPressedThisFrame) input.ValueRW.AltInteract.Set();

                input.ValueRW.Action1 = default;
                if (Keyboard.current.rKey.wasPressedThisFrame) input.ValueRW.Action1.Set();

                input.ValueRW.Action2 = default;
                if (Keyboard.current.tKey.wasPressedThisFrame) input.ValueRW.Action2.Set();

                input.ValueRW.Menu = default;
                if (Keyboard.current.escapeKey.wasPressedThisFrame) input.ValueRW.Menu.Set();

                float x = 0f;
                float y = 0f;

                if (Keyboard.current.dKey.isPressed) x += 1f;
                if (Keyboard.current.aKey.isPressed) x -= 1f;
                if (Keyboard.current.wKey.isPressed) y += 1f;
                if (Keyboard.current.sKey.isPressed) y -= 1f;

                input.ValueRW.Move = new float2(x, y);

                float2 mouseDelta = Mouse.current.delta.ReadValue();
                float2 lookDelta = mouseDelta * camera.ValueRO.LookSensitivity;

                camera.ValueRW.YawDegrees += lookDelta.x;
                camera.ValueRW.PitchDegrees -= lookDelta.y;
                camera.ValueRW.PitchDegrees = math.clamp((float)camera.ValueRO.PitchDegrees, (float)camera.ValueRO.MinPitch, (float)camera.ValueRO.MaxPitch);

                input.ValueRW.ViewYawDegrees = camera.ValueRO.YawDegrees;
                input.ValueRW.ViewPitchDegrees = camera.ValueRO.PitchDegrees;

                float scrollY = Mouse.current.scroll.ReadValue().y;
                input.ValueRW.ScrollDelta = scrollY > 0 ? 1 : scrollY < 0 ? -1 : 0;
            }
        }
    }
}
