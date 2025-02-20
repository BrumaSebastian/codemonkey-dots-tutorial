
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSignleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSignleton.CollisionWorld;
        NativeList<DistanceHit> distanceHitList = new(Allocator.Temp);

        foreach ((var localTransform, var findTarget, var target) 
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<FindTarget>,
                RefRW<Target>>())
        {
            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if (findTarget.ValueRO.timer > 0f)
            {
                continue;
            }

            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;

            distanceHitList.Clear();

            CollisionFilter collisionFilter = new()
            {
                GroupIndex = 0,
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNITS_LAYER,
            };

            if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.range, ref distanceHitList, collisionFilter))
            {
                foreach (var distanceHit in distanceHitList)
                {
                    Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);

                    if (targetUnit.faction == findTarget.ValueRO.targetFaction)
                    {
                        target.ValueRW.targetEntity = distanceHit.Entity;
                        break;
                    }
                }
            }
        }
    }
}
