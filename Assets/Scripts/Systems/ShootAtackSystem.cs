using Unity.Burst;
using Unity.Entities;

partial struct ShootAtackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((var shootAtack, var target) in SystemAPI.Query<RefRW<ShootAtack>, RefRO<Target>>())
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

            var targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
            int damageAmount = 1;
            targetHealth.ValueRW.healthAmount -= damageAmount;
        }
    }
}
