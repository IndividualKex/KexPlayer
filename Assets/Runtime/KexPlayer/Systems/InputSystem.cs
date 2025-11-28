using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace KexPlayer {
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [AlwaysSynchronizeSystem]
    public partial class InputSystem : SystemBase {
        private bool _focusLost;

        protected override void OnCreate() {
            Application.focusChanged += OnFocusChanged;
        }

        protected override void OnDestroy() {
            Application.focusChanged -= OnFocusChanged;
        }

        private void OnFocusChanged(bool hasFocus) {
            if (!hasFocus) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _focusLost = true;
            }
        }

        protected override void OnUpdate() {
            foreach (var (input, camera, cursorLock) in SystemAPI
                .Query<RefRW<Input>, RefRW<Camera>, RefRW<CursorLock>>()
                .WithAll<GhostOwnerIsLocal>()
            ) {
                bool wasLocked = cursorLock.ValueRO.Value;

                if (_focusLost) {
                    cursorLock.ValueRW.Value = false;
                    _focusLost = false;
                }

                if (Keyboard.current.escapeKey.wasPressedThisFrame) {
                    cursorLock.ValueRW.Value = false;
                }

                if (!cursorLock.ValueRO.Value && Mouse.current.leftButton.wasPressedThisFrame) {
                    bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                    if (!overUI) {
                        cursorLock.ValueRW.Value = true;
                    }
                }

                if (cursorLock.ValueRO.Value) {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else {
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

                input.ValueRW.Digit1 = default;
                input.ValueRW.Digit2 = default;
                input.ValueRW.Digit3 = default;
                input.ValueRW.Digit4 = default;
                input.ValueRW.Digit5 = default;
                input.ValueRW.Digit6 = default;
                input.ValueRW.Digit7 = default;
                input.ValueRW.Digit8 = default;
                input.ValueRW.Digit9 = default;
                input.ValueRW.Digit0 = default;
                if (Keyboard.current.digit1Key.wasPressedThisFrame) input.ValueRW.Digit1.Set();
                if (Keyboard.current.digit2Key.wasPressedThisFrame) input.ValueRW.Digit2.Set();
                if (Keyboard.current.digit3Key.wasPressedThisFrame) input.ValueRW.Digit3.Set();
                if (Keyboard.current.digit4Key.wasPressedThisFrame) input.ValueRW.Digit4.Set();
                if (Keyboard.current.digit5Key.wasPressedThisFrame) input.ValueRW.Digit5.Set();
                if (Keyboard.current.digit6Key.wasPressedThisFrame) input.ValueRW.Digit6.Set();
                if (Keyboard.current.digit7Key.wasPressedThisFrame) input.ValueRW.Digit7.Set();
                if (Keyboard.current.digit8Key.wasPressedThisFrame) input.ValueRW.Digit8.Set();
                if (Keyboard.current.digit9Key.wasPressedThisFrame) input.ValueRW.Digit9.Set();
                if (Keyboard.current.digit0Key.wasPressedThisFrame) input.ValueRW.Digit0.Set();

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
