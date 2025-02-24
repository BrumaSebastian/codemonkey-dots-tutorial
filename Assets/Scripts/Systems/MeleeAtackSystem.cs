using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct MeleeAtackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<RaycastHit> raycastHits = new NativeList<RaycastHit>();

        foreach ((var localTransform, var meleeAtack, var target, var unitMover) 
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<MeleeAtack>,
                RefRO<Target>,
                RefRW<UnitMover>>()
                .WithDisabled<MoveOverride>())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float meleeAtackDistanceSq = 2f;

            bool isCloseEnoughToAtack = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position) < meleeAtackDistanceSq;
            bool isTouchingTarget = false;

            if (!isCloseEnoughToAtack)
            {
                float3 directionToTarget = targetLocalTransform.Position - localTransform.ValueRO.Position;
                directionToTarget = math.normalize(directionToTarget);
                float distanceExtraToTestRaycast = .4f;
                RaycastInput raycastInput = new RaycastInput()
                {
                    Start = localTransform.ValueRO.Position,
                    End = localTransform.ValueRO.Position + directionToTarget * (meleeAtack.ValueRO.colliderSize + distanceExtraToTestRaycast),
                    Filter = CollisionFilter.Default,
                };

                raycastHits.Clear();

                if (collisionWorld.CastRay(raycastInput, ref raycastHits))
                {
                    foreach (var raycastHit in raycastHits)
                    {
                        if (raycastHit.Entity == target.ValueRO.targetEntity)
                        {
                            isTouchingTarget = true;
                            break;
                        }
                    }
                }
            }

            if (!isCloseEnoughToAtack && !isTouchingTarget)
            {
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
            }
            else
            {
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;

                meleeAtack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

                if (meleeAtack.ValueRO.timer > 0)
                {
                    continue;
                }

                meleeAtack.ValueRW.timer = meleeAtack.ValueRO.timerMax;

                var targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= meleeAtack.ValueRO.damageAmount;
                targetHealth.ValueRW.onHealthChanged = true;
            }
        }
    }
}
