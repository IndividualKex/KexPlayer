using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Unity.Collections;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerInitSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        var query = SystemAPI.QueryBuilder()
            .WithAll<ClientConnectionRequest, ReceiveRpcCommandRequest>()
            .Build();
        state.RequireForUpdate(query);

        state.RequireForUpdate<PlayerConfig>();
    }

    public void OnUpdate(ref SystemState state) {
        var config = SystemAPI.GetSingleton<PlayerConfig>();
        var worldName = state.WorldUnmanaged.Name;

        using var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (request, entity) in SystemAPI
            .Query<ReceiveRpcCommandRequest>()
            .WithAll<ClientConnectionRequest>()
            .WithEntityAccess()
        ) {
            var player = ecb.Instantiate(config.Prefab);

            var networkId = SystemAPI.GetComponent<NetworkId>(request.SourceConnection).Value;
            ecb.SetComponent(player, new GhostOwner { NetworkId = networkId });

            ecb.AppendToBuffer(request.SourceConnection, new LinkedEntityGroup { Value = player });
            ecb.AddComponent<NetworkStreamInGame>(request.SourceConnection);

            ecb.DestroyEntity(entity);

            Debug.Log($"Server: Spawned player for client {networkId}");
        }
        ecb.Playback(state.EntityManager);
    }
}
