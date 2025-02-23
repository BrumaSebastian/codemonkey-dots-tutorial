using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    public int healtAmount;
    public int healtAmountMax;

    public class Baker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Health()
            {
                healthAmount = authoring.healtAmount,
                healthAmountMax = authoring.healtAmountMax,
                onHealthChanged = true
            });
        }
    }
}

public struct Health : IComponentData
{
    public int healthAmount;
    public int healthAmountMax;
    public bool onHealthChanged;
}
