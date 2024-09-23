using UnityEngine;
using UnityEngine.Scripting;

namespace Entities
{
    [Preserve]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class CopyInitialTransformFromGameObjectSystem : HybridSystemBase<Transform, LocalToWorld, CopyInitialTransformFromGameObject>
    {
        protected override void OnStartRunning()
        {
            base.OnStartRunning(); 
            
            CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate(int index, Entity entity, Transform component1, LocalToWorld component2, CopyInitialTransformFromGameObject component3)
        {
            if (component1 != null)
                component2.Value = component1.localToWorldMatrix;

            CommandBuffer.RemoveComponentData<CopyInitialTransformFromGameObject>(entity);

            var position = EntityManager.GetComponentData<Position>(entity);
            if (position != null)
                position.Value = component1.position;

            var rotation = EntityManager.GetComponentData<Rotation>(entity);
            if (rotation != null)
                rotation.Value = component1.rotation;

            var scale = EntityManager.GetComponentData<Scale>(entity);
            if (scale != null)
                scale.Value = component1.lossyScale;
        }
    }
}
