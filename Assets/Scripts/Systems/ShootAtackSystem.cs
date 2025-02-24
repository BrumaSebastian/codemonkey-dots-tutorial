using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ShootAtackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((var localTransform, var shootAtack, var target, var unitMover) 
            in SystemAPI.Query<
                RefRW<LocalTransform>, 
                RefRW<ShootAtack>, 
                RefRO<Target>,
                RefRW<UnitMover>>()
                .WithDisabled<MoveOverride>())
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

            LocalTransform targetLocalTranform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            if (math.distance(localTransform.ValueRO.Position, targetLocalTranform.Position) > shootAtack.ValueRO.attackDistance)
            {
                unitMover.ValueRW.targetPosition = targetLocalTranform.Position;
                continue;
            }
            else
            {
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            }

            float3 aimDirection = targetLocalTranform.Position - localTransform.ValueRO.Position;
            aimDirection = math.normalize(aimDirection);

            quaternion targetRotation = quaternion.LookRotation(aimDirection, math.up());
            localTransform.ValueRW.Rotation =
                math.slerp(localTransform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);

            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
            float3 bulletSpawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAtack.ValueRO.bulletSpawnLocalPosition);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));

            RefRW<Bullet> bullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bullet.ValueRW.damageAmount = shootAtack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;

            shootAtack.ValueRW.onShoot.isTriggered = true;
            shootAtack.ValueRW.onShoot.shootFromPosition = bulletSpawnWorldPosition;
        }
    }
}
