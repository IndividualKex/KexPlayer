using UnityEngine;

public class FrameRateDebugger : MonoBehaviour {
    [SerializeField] private int targetFrameRate = 30;
    [SerializeField] private bool enableLimiting = true;
    [SerializeField] private bool showDebugInfo = true;

    private void Awake() {
        ApplyFrameRateLimit();
    }

    private void OnValidate() {
        if (Application.isPlaying) {
            ApplyFrameRateLimit();
        }
    }

    private void ApplyFrameRateLimit() {
        Application.targetFrameRate = enableLimiting ? targetFrameRate : -1;

        if (showDebugInfo) {
            Debug.Log($"[FrameRateDebugger] Target FPS: {(enableLimiting ? targetFrameRate.ToString() : "Unlimited")} | Fixed Timestep: {Time.fixedDeltaTime}s ({1f / Time.fixedDeltaTime:F1} Hz)");
        }
    }

#if UNITY_EDITOR
    private void OnGUI() {
        if (!showDebugInfo) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 120));
        GUILayout.Box($"FPS: {1f / Time.deltaTime:F1}\n" +
                     $"Frame Time: {Time.deltaTime * 1000:F2}ms\n" +
                     $"Fixed Timestep: {Time.fixedDeltaTime * 1000:F2}ms ({1f / Time.fixedDeltaTime:F1} Hz)\n" +
                     $"Target FPS: {(enableLimiting ? targetFrameRate.ToString() : "Unlimited")}");
        GUILayout.EndArea();
    }
#endif
}
