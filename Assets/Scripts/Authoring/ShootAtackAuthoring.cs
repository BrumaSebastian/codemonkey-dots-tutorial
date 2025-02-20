using Unity.Entities;
using UnityEngine;

public class ShootAtackAuthoring : MonoBehaviour
{
    public float timerMax;

    public class Baker : Baker<ShootAtackAuthoring>
    {
        public override void Bake(ShootAtackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootAtack()
            {
                timerMax = authoring.timerMax,
            });
        }
    }
}

public struct ShootAtack : IComponentData
{
    public float timer;
    public float timerMax;
}