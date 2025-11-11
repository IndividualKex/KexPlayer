using Unity.Entities;

namespace KexPlayer {
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(CameraSystem))]
    public partial class CameraApplySystem : SystemBase {
        protected override void OnUpdate() {
            if (UnityEngine.Camera.main == null) return;

            foreach (var camera in SystemAPI.Query<Camera>()) {
                UnityEngine.Camera.main.transform.SetPositionAndRotation(
                    camera.Position,
                    camera.Rotation
                );
                break;
            }
        }
    }
}
