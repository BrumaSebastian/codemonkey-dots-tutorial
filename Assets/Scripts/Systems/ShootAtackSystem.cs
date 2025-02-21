using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ShootAtackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((var localTransform, var shootAtack, var target) 
            in SystemAPI.Query<
                RefRO<LocalTransform>, 
                RefRW<ShootAtack>, 
                RefRO<Target>>())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }

            shootAtack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if (shootAtack.ValueRO.timer > 0f)
            {
                continue;
            }

            shootAtack.ValueRW.timer = shootAtack.ValueRO.timerMax;
            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            RefRW<Bullet> bullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bullet.ValueRW.damageAmount = shootAtack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
        }
    }
}
