using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomWalkingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((var randomWalking, var unitMover, var localTransform) 
            in SystemAPI.Query<
                RefRW<RandomWalking>,
                RefRW<UnitMover>,
                RefRO<LocalTransform>>())
        {
            if (math.distance(localTransform.ValueRO.Position, randomWalking.ValueRO.targetPosition) < UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
            {
                Random random = randomWalking.ValueRO.random;
                float3 randomDirection = new(random.NextFloat(-1f, 1f), 0 , random.NextFloat(-1f, 1f));
                randomDirection = math.normalize(randomDirection);
                randomWalking.ValueRW.targetPosition =
                    randomWalking.ValueRO.originPosition + randomDirection * random.NextFloat(randomWalking.ValueRO.distanceMin, randomWalking.ValueRO.distanceMax);

                randomWalking.ValueRW.random = random;
            }
            else
            {
                unitMover.ValueRW.targetPosition = randomWalking.ValueRW.targetPosition;
            }
        }
    }
}
