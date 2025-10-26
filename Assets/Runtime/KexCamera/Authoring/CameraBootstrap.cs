using UnityEngine;

namespace KexCamera {
    public class CameraBootstrap : MonoBehaviour {
        private void Awake() {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
