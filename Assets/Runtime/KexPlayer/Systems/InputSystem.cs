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
            double elapsedTime = SystemAPI.Time.ElapsedTime;
            float bufferDuration = InputBuffer.DefaultDuration;

            foreach (var (input, camera, cursorLock, buffer, entity) in SystemAPI
                .Query<RefRW<Input>, RefRW<Camera>, RefRW<CursorLock>, RefRW<InputBuffer>>()
                .WithAll<GhostOwnerIsLocal>()
                .WithEntityAccess()
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

                if (Keyboard.current.spaceKey.wasPressedThisFrame) buffer.ValueRW.JumpTime = elapsedTime;
                if (Keyboard.current.leftCtrlKey.wasPressedThisFrame) buffer.ValueRW.CrouchTime = elapsedTime;
                if (Keyboard.current.leftShiftKey.wasPressedThisFrame) buffer.ValueRW.SprintTime = elapsedTime;
                if (!justLocked && Mouse.current.leftButton.wasPressedThisFrame) buffer.ValueRW.FireTime = elapsedTime;
                if (Mouse.current.rightButton.wasPressedThisFrame) buffer.ValueRW.AltFireTime = elapsedTime;
                if (Keyboard.current.eKey.wasPressedThisFrame) buffer.ValueRW.InteractTime = elapsedTime;
                if (Keyboard.current.fKey.wasPressedThisFrame) buffer.ValueRW.AltInteractTime = elapsedTime;
                if (Keyboard.current.rKey.wasPressedThisFrame) buffer.ValueRW.Action1Time = elapsedTime;
                if (Keyboard.current.tKey.wasPressedThisFrame) buffer.ValueRW.Action2Time = elapsedTime;
                if (Keyboard.current.escapeKey.wasPressedThisFrame) buffer.ValueRW.MenuTime = elapsedTime;
                if (Keyboard.current.digit1Key.wasPressedThisFrame) buffer.ValueRW.Digit1Time = elapsedTime;
                if (Keyboard.current.digit2Key.wasPressedThisFrame) buffer.ValueRW.Digit2Time = elapsedTime;
                if (Keyboard.current.digit3Key.wasPressedThisFrame) buffer.ValueRW.Digit3Time = elapsedTime;
                if (Keyboard.current.digit4Key.wasPressedThisFrame) buffer.ValueRW.Digit4Time = elapsedTime;
                if (Keyboard.current.digit5Key.wasPressedThisFrame) buffer.ValueRW.Digit5Time = elapsedTime;
                if (Keyboard.current.digit6Key.wasPressedThisFrame) buffer.ValueRW.Digit6Time = elapsedTime;
                if (Keyboard.current.digit7Key.wasPressedThisFrame) buffer.ValueRW.Digit7Time = elapsedTime;
                if (Keyboard.current.digit8Key.wasPressedThisFrame) buffer.ValueRW.Digit8Time = elapsedTime;
                if (Keyboard.current.digit9Key.wasPressedThisFrame) buffer.ValueRW.Digit9Time = elapsedTime;
                if (Keyboard.current.digit0Key.wasPressedThisFrame) buffer.ValueRW.Digit0Time = elapsedTime;

                input.ValueRW.Jump = default;
                input.ValueRW.Crouch = default;
                input.ValueRW.Sprint = default;
                input.ValueRW.Fire = default;
                input.ValueRW.AltFire = default;
                input.ValueRW.Interact = default;
                input.ValueRW.AltInteract = default;
                input.ValueRW.Action1 = default;
                input.ValueRW.Action2 = default;
                input.ValueRW.Menu = default;
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

                if ((elapsedTime - buffer.ValueRO.JumpTime) <= bufferDuration) input.ValueRW.Jump.Set();
                if ((elapsedTime - buffer.ValueRO.CrouchTime) <= bufferDuration) input.ValueRW.Crouch.Set();
                if ((elapsedTime - buffer.ValueRO.SprintTime) <= bufferDuration) input.ValueRW.Sprint.Set();
                if ((elapsedTime - buffer.ValueRO.FireTime) <= bufferDuration) input.ValueRW.Fire.Set();
                if ((elapsedTime - buffer.ValueRO.AltFireTime) <= bufferDuration) input.ValueRW.AltFire.Set();
                if ((elapsedTime - buffer.ValueRO.InteractTime) <= bufferDuration) input.ValueRW.Interact.Set();
                if ((elapsedTime - buffer.ValueRO.AltInteractTime) <= bufferDuration) input.ValueRW.AltInteract.Set();
                if ((elapsedTime - buffer.ValueRO.Action1Time) <= bufferDuration) input.ValueRW.Action1.Set();
                if ((elapsedTime - buffer.ValueRO.Action2Time) <= bufferDuration) input.ValueRW.Action2.Set();
                if ((elapsedTime - buffer.ValueRO.MenuTime) <= bufferDuration) input.ValueRW.Menu.Set();
                if ((elapsedTime - buffer.ValueRO.Digit1Time) <= bufferDuration) input.ValueRW.Digit1.Set();
                if ((elapsedTime - buffer.ValueRO.Digit2Time) <= bufferDuration) input.ValueRW.Digit2.Set();
                if ((elapsedTime - buffer.ValueRO.Digit3Time) <= bufferDuration) input.ValueRW.Digit3.Set();
                if ((elapsedTime - buffer.ValueRO.Digit4Time) <= bufferDuration) input.ValueRW.Digit4.Set();
                if ((elapsedTime - buffer.ValueRO.Digit5Time) <= bufferDuration) input.ValueRW.Digit5.Set();
                if ((elapsedTime - buffer.ValueRO.Digit6Time) <= bufferDuration) input.ValueRW.Digit6.Set();
                if ((elapsedTime - buffer.ValueRO.Digit7Time) <= bufferDuration) input.ValueRW.Digit7.Set();
                if ((elapsedTime - buffer.ValueRO.Digit8Time) <= bufferDuration) input.ValueRW.Digit8.Set();
                if ((elapsedTime - buffer.ValueRO.Digit9Time) <= bufferDuration) input.ValueRW.Digit9.Set();
                if ((elapsedTime - buffer.ValueRO.Digit0Time) <= bufferDuration) input.ValueRW.Digit0.Set();

                float x = 0f;
                float y = 0f;

                if (Keyboard.current.dKey.isPressed) x += 1f;
                if (Keyboard.current.aKey.isPressed) x -= 1f;
                if (Keyboard.current.wKey.isPressed) y += 1f;
                if (Keyboard.current.sKey.isPressed) y -= 1f;

                input.ValueRW.Move = new float2(x, y);

                bool canLook = true;
                if (SystemAPI.HasComponent<PlayerCapabilities>(entity)) {
                    canLook = SystemAPI.GetComponent<PlayerCapabilities>(entity).CanLook;
                }

                if (canLook) {
                    float2 mouseDelta = Mouse.current.delta.ReadValue();
                    float2 lookDelta = mouseDelta * camera.ValueRO.LookSensitivity;

                    camera.ValueRW.YawDegrees += lookDelta.x;
                    camera.ValueRW.PitchDegrees -= lookDelta.y;
                    camera.ValueRW.PitchDegrees = math.clamp((float)camera.ValueRO.PitchDegrees, (float)camera.ValueRO.MinPitch, (float)camera.ValueRO.MaxPitch);
                }

                input.ValueRW.ViewYawDegrees = camera.ValueRO.YawDegrees;
                input.ValueRW.ViewPitchDegrees = camera.ValueRO.PitchDegrees;

                float scrollY = Mouse.current.scroll.ReadValue().y;
                if (scrollY > 0) buffer.ValueRW.ScrollUpTime = elapsedTime;
                else if (scrollY < 0) buffer.ValueRW.ScrollDownTime = elapsedTime;

                input.ValueRW.ScrollUp = default;
                input.ValueRW.ScrollDown = default;
                if ((elapsedTime - buffer.ValueRO.ScrollUpTime) <= bufferDuration) input.ValueRW.ScrollUp.Set();
                if ((elapsedTime - buffer.ValueRO.ScrollDownTime) <= bufferDuration) input.ValueRW.ScrollDown.Set();
            }
        }
    }
}
