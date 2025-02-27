using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct ShootLightSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach (var shootAtack in SystemAPI.Query<RefRO<ShootAtack>>())
        {
            if (shootAtack.ValueRO.onShoot.isTriggered)
            {
                Entity shootLightEntity = state.EntityManager.Instantiate(entitiesReferences.shootLightPrefabEntity);
                SystemAPI.SetComponent(shootLightEntity, LocalTransform.FromPosition(shootAtack.ValueRO.onShoot.shootFromPosition));
            }
        }
    }
}
