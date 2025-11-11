using UnityEngine;

namespace KexPlayer {
    public class CameraBootstrap : MonoBehaviour {
        private void Awake() {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
