using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((var localTransform, var bullet, var target, var entity) 
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<Bullet>,
                RefRO<Target>>().WithEntityAccess())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);

            float3 moveDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

            float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);
            float destroyDistance = .2f;

            if (distanceAfterSq > distanceBeforeSq)
            {
                localTransform.ValueRW.Position = targetLocalTransform.Position;
            }

            if (math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position) < destroyDistance)
            {
                var targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}
