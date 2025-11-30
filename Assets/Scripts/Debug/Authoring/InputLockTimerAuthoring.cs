using Unity.Entities;
using UnityEngine;

public class InputLockTimerAuthoring : MonoBehaviour {
    public class Baker : Baker<InputLockTimerAuthoring> {
        public override void Bake(InputLockTimerAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<InputLockTimer>(entity);
        }
    }
}
