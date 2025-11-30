using Unity.Entities;

namespace KexPlayer {
    public enum InputBufferField {
        Jump,
        Crouch,
        Sprint,
        Fire,
        AltFire,
        Interact,
        AltInteract,
        Action1,
        Action2,
        Menu,
        Digit1,
        Digit2,
        Digit3,
        Digit4,
        Digit5,
        Digit6,
        Digit7,
        Digit8,
        Digit9,
        Digit0,
        ScrollUp,
        ScrollDown,
    }

    public struct InputBuffer : IComponentData {
        public double JumpTime;
        public double CrouchTime;
        public double SprintTime;
        public double FireTime;
        public double AltFireTime;
        public double InteractTime;
        public double AltInteractTime;
        public double Action1Time;
        public double Action2Time;
        public double MenuTime;
        public double Digit1Time;
        public double Digit2Time;
        public double Digit3Time;
        public double Digit4Time;
        public double Digit5Time;
        public double Digit6Time;
        public double Digit7Time;
        public double Digit8Time;
        public double Digit9Time;
        public double Digit0Time;
        public double ScrollUpTime;
        public double ScrollDownTime;

        public const float DefaultDuration = 0.15f;

        public void Clear(InputBufferField field) {
            switch (field) {
                case InputBufferField.Jump: JumpTime = double.MinValue; break;
                case InputBufferField.Crouch: CrouchTime = double.MinValue; break;
                case InputBufferField.Sprint: SprintTime = double.MinValue; break;
                case InputBufferField.Fire: FireTime = double.MinValue; break;
                case InputBufferField.AltFire: AltFireTime = double.MinValue; break;
                case InputBufferField.Interact: InteractTime = double.MinValue; break;
                case InputBufferField.AltInteract: AltInteractTime = double.MinValue; break;
                case InputBufferField.Action1: Action1Time = double.MinValue; break;
                case InputBufferField.Action2: Action2Time = double.MinValue; break;
                case InputBufferField.Menu: MenuTime = double.MinValue; break;
                case InputBufferField.Digit1: Digit1Time = double.MinValue; break;
                case InputBufferField.Digit2: Digit2Time = double.MinValue; break;
                case InputBufferField.Digit3: Digit3Time = double.MinValue; break;
                case InputBufferField.Digit4: Digit4Time = double.MinValue; break;
                case InputBufferField.Digit5: Digit5Time = double.MinValue; break;
                case InputBufferField.Digit6: Digit6Time = double.MinValue; break;
                case InputBufferField.Digit7: Digit7Time = double.MinValue; break;
                case InputBufferField.Digit8: Digit8Time = double.MinValue; break;
                case InputBufferField.Digit9: Digit9Time = double.MinValue; break;
                case InputBufferField.Digit0: Digit0Time = double.MinValue; break;
                case InputBufferField.ScrollUp: ScrollUpTime = double.MinValue; break;
                case InputBufferField.ScrollDown: ScrollDownTime = double.MinValue; break;
            }
        }
    }
}
