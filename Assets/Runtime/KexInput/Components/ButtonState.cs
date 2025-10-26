using Unity.Mathematics;

namespace KexInput {
    public struct ButtonState {
        private const double DEFAULT_HOLD_THRESHOLD = 0.2;
        private const double DEFAULT_BUFFER_WINDOW = 0.15;

        private double _pressTime;
        private double _releaseTime;
        private byte _isPressed;
        private byte _wasConsumed;

        public void Press(double time) {
            if (_isPressed == 0) {
                _pressTime = time;
                _releaseTime = 0;
                _wasConsumed = 0;
            }
            _isPressed = 1;
        }

        public void Release(double time) {
            if (_isPressed == 1) {
                _releaseTime = time;
            }
            _isPressed = 0;
        }

        public void Consume() {
            _wasConsumed = 1;
        }

        public bool IsPressed() {
            return _isPressed == 1;
        }

        public bool WasConsumed() {
            return _wasConsumed == 1;
        }

        public double GetPressDuration(double time) {
            if (_isPressed == 0) return 0;
            return time - _pressTime;
        }

        public double GetTimeSincePress(double time) {
            return time - _pressTime;
        }

        public double GetTimeSinceRelease(double time) {
            return time - _releaseTime;
        }

        public bool WasJustPressed(double time, double bufferWindow = DEFAULT_BUFFER_WINDOW) {
            if (_wasConsumed == 1) return false;
            if (_pressTime <= 0) return false;

            double timeSincePress = time - _pressTime;
            return timeSincePress >= 0 && timeSincePress <= bufferWindow;
        }

        public bool WasJustReleased(double time, double bufferWindow = DEFAULT_BUFFER_WINDOW) {
            if (_wasConsumed == 1) return false;
            if (_releaseTime <= 0) return false;

            double timeSinceRelease = time - _releaseTime;
            return timeSinceRelease >= 0 && timeSinceRelease <= bufferWindow;
        }

        public bool WasClicked(double time, double holdThreshold = DEFAULT_HOLD_THRESHOLD, double releaseBuffer = DEFAULT_BUFFER_WINDOW) {
            if (_wasConsumed == 1) return false;
            if (_releaseTime <= 0 || _releaseTime <= _pressTime) return false;

            double timeSinceRelease = time - _releaseTime;
            double pressDuration = _releaseTime - _pressTime;

            return timeSinceRelease <= releaseBuffer && pressDuration < holdThreshold;
        }

        public bool IsHeldFor(double time, double holdThreshold = DEFAULT_HOLD_THRESHOLD) {
            if (_wasConsumed == 1) return false;
            if (_isPressed == 0) return false;

            return GetPressDuration(time) >= holdThreshold;
        }

        public float GetHoldProgress(double time, double holdThreshold = DEFAULT_HOLD_THRESHOLD) {
            if (_isPressed == 0) return 0f;

            double elapsed = time - _pressTime;
            return math.saturate((float)(elapsed / holdThreshold));
        }
    }
}