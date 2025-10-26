using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace KexInput {
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class InputSystem : SystemBase {
        protected override void OnUpdate() {
            double currentTime = SystemAPI.Time.ElapsedTime;

            foreach (var inputRef in SystemAPI.Query<RefRW<Input>>()) {
                ref var input = ref inputRef.ValueRW;

                if (Keyboard.current.spaceKey.isPressed) {
                    input.Jump.Press(currentTime);
                }
                else {
                    input.Jump.Release(currentTime);
                }

                if (Keyboard.current.escapeKey.isPressed) {
                    input.Menu.Press(currentTime);
                }
                else {
                    input.Menu.Release(currentTime);
                }

                if (Mouse.current.leftButton.isPressed) {
                    input.Action1.Press(currentTime);
                }
                else {
                    input.Action1.Release(currentTime);
                }

                if (Mouse.current.rightButton.isPressed) {
                    input.Action2.Press(currentTime);
                }
                else {
                    input.Action2.Release(currentTime);
                }

                if (Keyboard.current.fKey.isPressed) {
                    input.Action3.Press(currentTime);
                }
                else {
                    input.Action3.Release(currentTime);
                }

                if (Keyboard.current.gKey.isPressed) {
                    input.Action4.Press(currentTime);
                }
                else {
                    input.Action4.Release(currentTime);
                }

                if (Keyboard.current.rKey.isPressed) {
                    input.Action5.Press(currentTime);
                }
                else {
                    input.Action5.Release(currentTime);
                }

                if (Keyboard.current.yKey.isPressed) {
                    input.Action6.Press(currentTime);
                }
                else {
                    input.Action6.Release(currentTime);
                }

                if (Keyboard.current.leftCtrlKey.isPressed) {
                    input.Action7.Press(currentTime);
                }
                else {
                    input.Action7.Release(currentTime);
                }

                if (Keyboard.current.leftAltKey.isPressed) {
                    input.Action8.Press(currentTime);
                }
                else {
                    input.Action8.Release(currentTime);
                }

                float x = 0f;
                float y = 0f;

                if (Keyboard.current.dKey.isPressed) x += 1f;
                if (Keyboard.current.aKey.isPressed) x -= 1f;
                if (Keyboard.current.wKey.isPressed) y += 1f;
                if (Keyboard.current.sKey.isPressed) y -= 1f;

                input.Move = new float2(x, y);
                input.Look = Mouse.current.delta.ReadValue();

                input.ScrollDelta = Mouse.current.scroll.ReadValue().y;
            }
        }
    }
}