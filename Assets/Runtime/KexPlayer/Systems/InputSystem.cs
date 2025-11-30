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
            RequireForUpdate<NetworkTime>();
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
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var currentTick = networkTime.ServerTick;

            foreach (var (input, camera, cursorLock, entity) in SystemAPI
                .Query<RefRW<Input>, RefRW<Camera>, RefRW<CursorLock>>()
                .WithAll<GhostOwnerIsLocal>()
                .WithEntityAccess()
            ) {
                bool inputLocked = SystemAPI.HasComponent<InputLockTimer>(entity)
                    && SystemAPI.GetComponent<InputLockTimer>(entity).IsLocked(currentTick);

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

                input.ValueRW = default;

                if (Keyboard.current.spaceKey.wasPressedThisFrame) input.ValueRW.Jump.Set();
                if (Keyboard.current.leftCtrlKey.wasPressedThisFrame) input.ValueRW.Crouch.Set();
                if (Keyboard.current.leftShiftKey.wasPressedThisFrame) input.ValueRW.Sprint.Set();
                if (!justLocked && Mouse.current.leftButton.wasPressedThisFrame) input.ValueRW.Fire.Set();
                input.ValueRW.FireHeld = Mouse.current.leftButton.isPressed;
                if (Mouse.current.rightButton.wasPressedThisFrame) input.ValueRW.AltFire.Set();
                input.ValueRW.AltFireHeld = Mouse.current.rightButton.isPressed;
                if (Keyboard.current.eKey.wasPressedThisFrame) input.ValueRW.Interact.Set();
                if (Keyboard.current.fKey.wasPressedThisFrame) input.ValueRW.AltInteract.Set();
                if (Keyboard.current.rKey.wasPressedThisFrame) input.ValueRW.Action1.Set();
                if (Keyboard.current.tKey.wasPressedThisFrame) input.ValueRW.Action2.Set();
                if (Keyboard.current.escapeKey.wasPressedThisFrame) input.ValueRW.Menu.Set();
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

                if (!inputLocked) {
                    float2 mouseDelta = Mouse.current.delta.ReadValue();
                    float2 lookDelta = mouseDelta * camera.ValueRO.LookSensitivity;

                    camera.ValueRW.YawDegrees += lookDelta.x;
                    camera.ValueRW.PitchDegrees -= lookDelta.y;
                    camera.ValueRW.PitchDegrees = math.clamp((float)camera.ValueRO.PitchDegrees, (float)camera.ValueRO.MinPitch, (float)camera.ValueRO.MaxPitch);
                }

                input.ValueRW.ViewYawDegrees = camera.ValueRO.YawDegrees;
                input.ValueRW.ViewPitchDegrees = camera.ValueRO.PitchDegrees;

                float scrollY = Mouse.current.scroll.ReadValue().y;
                if (scrollY > 0) input.ValueRW.ScrollUp.Set();
                else if (scrollY < 0) input.ValueRW.ScrollDown.Set();
            }
        }
    }
}
