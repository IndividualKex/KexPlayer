using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Graphics;
using KexInteract;

namespace KexOutline {
    [BurstCompile]
    public partial struct OutlineSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<OutlineConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var config = SystemAPI.GetSingleton<OutlineConfig>();

            using var interacted = new NativeHashSet<Entity>(16, Allocator.Temp);
            foreach (var interactTarget in SystemAPI.Query<RefRO<Interacter>>()) {
                if (interactTarget.ValueRO.Target == Entity.Null) continue;
                interacted.Add(interactTarget.ValueRO.Target);
            }

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            using var map = new NativeHashMap<Entity, int>(16, Allocator.Temp);
            foreach (var (outlineRenderer, entity) in SystemAPI.Query<OutlineRenderer>().WithEntityAccess().WithAll<Interactable>()) {
                Entity renderer = outlineRenderer.Renderer;
                int layer = interacted.Contains(entity) ? config.OutlineLayer : outlineRenderer.RendererLayer;
                map.Add(entity, layer);

                if (renderer == Entity.Null || !state.EntityManager.HasComponent<RenderFilterSettings>(renderer)) continue;

                var renderFilterSettings = state.EntityManager.GetSharedComponent<RenderFilterSettings>(renderer);
                renderFilterSettings.Layer = layer;
                ecb.SetSharedComponent(renderer, renderFilterSettings);
            }

            foreach (var (linkedOutline, entity) in SystemAPI.Query<LinkedOutline>().WithEntityAccess()) {
                if (!map.TryGetValue(linkedOutline.Value, out int layer) ||
                    !state.EntityManager.HasComponent<RenderFilterSettings>(entity)) continue;

                var renderFilterSettings = state.EntityManager.GetSharedComponent<RenderFilterSettings>(entity);
                renderFilterSettings.Layer = layer;
                ecb.SetSharedComponent(entity, renderFilterSettings);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
