using Unity.Burst;
using Unity.Entities;
using UnityEngine;

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
            Debug.Log("shoot");
        }
    }
}
