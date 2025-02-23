using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ShootAtackAuthoring : MonoBehaviour
{
    public float timerMax;
    public int damageAmount;
    public float attackDistance;
    public Transform bulletSpawnPositionTransform;

    public class Baker : Baker<ShootAtackAuthoring>
    {
        public override void Bake(ShootAtackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootAtack()
            {
                timerMax = authoring.timerMax,
                damageAmount = authoring.damageAmount,
                attackDistance = authoring.attackDistance,
                bulletSpawnLocalPosition = authoring.bulletSpawnPositionTransform.localPosition
            });
        }
    }
}

public struct ShootAtack : IComponentData
{
    public float timer;
    public float timerMax;
    public int damageAmount;
    public float attackDistance;
    public float3 bulletSpawnLocalPosition;
}