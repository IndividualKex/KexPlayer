using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientInitSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        var query = SystemAPI.QueryBuilder()
            .WithAll<NetworkId>()
            .WithNone<NetworkStreamInGame>()
            .Build();
        state.RequireForUpdate(query);
    }

    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (id, entity) in SystemAPI
            .Query<NetworkId>()
            .WithNone<NetworkStreamInGame>()
            .WithEntityAccess()
        ) {
            ecb.AddComponent<NetworkStreamInGame>(entity);

            var request = ecb.CreateEntity();
            ecb.AddComponent<ClientConnectionRequest>(request);
            ecb.AddComponent(request, new SendRpcCommandRequest { TargetConnection = entity });

            UnityEngine.Debug.Log($"Client: Sent join request to server for client {id}");
        }
        ecb.Playback(state.EntityManager);
    }
}
