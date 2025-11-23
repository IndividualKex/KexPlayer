using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.NetCode;

namespace KexPlayer {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PredictedSimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [BurstCompile]
    public partial struct HeadApplySystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (head, transform) in SystemAPI
                .Query<Head, RefRW<LocalTransform>>()
            ) {
                if (!SystemAPI.HasComponent<HeadRotation>(head.Player)) continue;
                var headRotation = SystemAPI.GetComponent<HeadRotation>(head.Player);
                transform.ValueRW = LocalTransform.FromRotation(headRotation.LocalRotation);
            }
        }
    }
}
