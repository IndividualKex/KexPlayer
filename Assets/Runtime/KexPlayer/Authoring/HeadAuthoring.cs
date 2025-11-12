using Unity.Entities;
using UnityEngine;

namespace KexPlayer {
    public class HeadAuthoring : MonoBehaviour {
        public PlayerAuthoring Player;

        private class Baker : Baker<HeadAuthoring> {
            public override void Bake(HeadAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var playerEntity = GetEntity(authoring.Player.gameObject, TransformUsageFlags.Dynamic);
                AddComponent(entity, new Head {
                    Player = playerEntity,
                });
            }
        }
    }
}
