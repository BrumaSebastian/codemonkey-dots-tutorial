using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct UnitMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((var localTransform, var moveSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>>())
        {
            localTransform.ValueRW.Position = localTransform.ValueRO.Position + new float3(1, 0, 0) * SystemAPI.Time.DeltaTime;
        }
    }
}
