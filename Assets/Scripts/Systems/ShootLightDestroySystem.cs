using Unity.Burst;
using Unity.Entities;

partial struct ShootLightDestroySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commandBuffer = 
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((var shootlight,var entity) in SystemAPI.Query<RefRW<ShootLight>>().WithEntityAccess())
        {
            shootlight.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if (shootlight.ValueRO.timer < 0f)
            {
                commandBuffer.DestroyEntity(entity);
            }
        }
    }
}
