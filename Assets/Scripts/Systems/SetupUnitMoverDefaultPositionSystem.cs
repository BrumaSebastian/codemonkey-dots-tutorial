using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SetupUnitMoverDefaultPositionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((var localTransform, var unitMover, var setupUnitMoverDefaultPosition, var entity) 
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<UnitMover>,
                RefRO<SetupUnitMoverDefaultPosition>>().WithEntityAccess())
        {
            unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            entityCommandBuffer.RemoveComponent<SetupUnitMoverDefaultPosition>(entity);
        }
    }
}
