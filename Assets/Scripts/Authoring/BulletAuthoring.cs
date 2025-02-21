using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    public float speed;
    public int damageAmount;

    public class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Bullet() 
            { 
                speed = authoring.speed,
                damageAmount = authoring.damageAmount
            });
        }
    }
}

public struct Bullet : IComponentData
{
    public float speed;
    public int damageAmount;
}
