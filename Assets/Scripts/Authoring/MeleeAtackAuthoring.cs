using Unity.Entities;
using UnityEngine;

public class MeleeAtackAuthoring : MonoBehaviour
{
    public float timerMax;
    public int damageAmount;
    public float colliderSize;

    public class Baker : Baker<MeleeAtackAuthoring>
    {
        public override void Bake(MeleeAtackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MeleeAtack
            {
                timerMax = authoring.timerMax,
                damageAmount = authoring.damageAmount,
                colliderSize = authoring.colliderSize
            });
        }
    }
}

public struct MeleeAtack : IComponentData
{
    public float timer;
    public float timerMax;
    public int damageAmount;
    public float colliderSize;
}
