using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnitMoverJob unitMoverJobs = new ()
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        unitMoverJobs.ScheduleParallel();

        //foreach ((var localTransform, var unitMover, var physicsVelocity) 
        //    in SystemAPI.Query<
        //        RefRW<LocalTransform>,
        //        RefRO<UnitMover>,
        //        RefRW<PhysicsVelocity>>())
        //{
        //    float3 targetPosition = MouseWorldPosition.Instance.GetPosition();
        //    float3 moveDirection = unitMover.ValueRO.targetPosition - localTransform.ValueRO.Position;
        //    moveDirection = math.normalize(moveDirection);

        //    localTransform.ValueRW.Rotation = math.slerp(
        //        localTransform.ValueRO.Rotation, 
        //        quaternion.LookRotation(moveDirection, math.up()), 
        //        SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);
        //    physicsVelocity.ValueRW.Linear = moveDirection * unitMover.ValueRO.moveSpeed;
        //    physicsVelocity.ValueRW.Angular = float3.zero;
        //    //localTransform.ValueRW.Position += moveSpeed.ValueRO.value * SystemAPI.Time.DeltaTime * moveDirection;
        //}
    }
}


[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float deltaTime;

    public void Execute (ref LocalTransform localTransform, in UnitMover unitMover, ref PhysicsVelocity physicsVelocity)
    {
        float3 moveDirection = unitMover.targetPosition - localTransform.Position;

        float reachedTargetDistanceSq = 2f;

        if (math.lengthsq(moveDirection) < reachedTargetDistanceSq)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }

        moveDirection = math.normalize(moveDirection);

        localTransform.Rotation = math.slerp(
            localTransform.Rotation,
            quaternion.LookRotation(moveDirection, math.up()),
            deltaTime * unitMover.rotationSpeed);
        physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;
        physicsVelocity.Angular = float3.zero;
    }
}
