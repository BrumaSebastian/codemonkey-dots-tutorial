using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ZombieSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((var localTransform, var zombieSpawner) 
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<ZombieSpawner>>())
        {
            zombieSpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if (zombieSpawner.ValueRO.timer > 0f)
            {
                continue;
            }

            zombieSpawner.ValueRW.timer = zombieSpawner.ValueRO.timerMax;

            Entity zombieEntity = state.EntityManager.Instantiate(entitiesReferences.zombiePrefabEntity);
            SystemAPI.SetComponent(zombieEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
        }
    }
}
