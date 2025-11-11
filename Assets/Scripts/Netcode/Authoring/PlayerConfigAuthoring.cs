using UnityEngine;
using Unity.Entities;

public class PlayerConfigAuthoring : MonoBehaviour {
    public GameObject Prefab;

    private class Baker : Baker<PlayerConfigAuthoring> {
        public override void Bake(PlayerConfigAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerConfig {
                Prefab = prefab,
            });
        }
    }

}
