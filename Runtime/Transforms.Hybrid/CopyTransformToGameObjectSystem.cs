using UnityEngine;
using UnityEngine.Scripting;

namespace Entities
{
    [Preserve]
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(TRSToLocalToWorldSystem))]
    public class CopyTransformToGameObjectSystem : HybridSystemBase<Transform, LocalToWorld, CopyTransformToGameObject>
    {
        protected override void OnUpdate(int index, Entity entity, Transform component1, LocalToWorld component2, CopyTransformToGameObject component3)
        {
            if (component1 != null)
            {
                component1.position = component2.Position;
                component1.rotation = component2.Rotation;
                component1.localScale = component2.Scale;
            }
        }
    }
}
