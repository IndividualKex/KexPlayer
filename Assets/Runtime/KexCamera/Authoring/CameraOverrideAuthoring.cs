using Unity.Entities;
using UnityEngine;

namespace KexCamera {
    public class CameraOverrideAuthoring : MonoBehaviour {
        private class Baker : Baker<CameraOverrideAuthoring> {
            public override void Bake(CameraOverrideAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CameraOverride {
                    Position = authoring.transform.position,
                    Rotation = authoring.transform.rotation,
                });
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 0.2f);
            Gizmos.DrawLine(Vector3.zero, Vector3.forward * 1f);
        }
    }
}
