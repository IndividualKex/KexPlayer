using KexPlayer;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Graphics;

namespace KexOutline {
    [BurstCompile]
    public partial struct OutlineSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<OutlineConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var config = SystemAPI.GetSingleton<OutlineConfig>();

            using var targeted = new NativeHashSet<Entity>(16, Allocator.Temp);
            foreach (var target in SystemAPI.Query<RefRO<Target>>()) {
                if (target.ValueRO.Value == Entity.Null) continue;
                targeted.Add(target.ValueRO.Value);
            }

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            using var map = new NativeHashMap<Entity, int>(16, Allocator.Temp);
            foreach (var (outlineRenderer, entity) in SystemAPI.Query<OutlineRenderer>().WithEntityAccess()) {
                Entity renderer = outlineRenderer.Renderer;
                int layer = targeted.Contains(entity) ? config.OutlineLayer : outlineRenderer.RendererLayer;
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
