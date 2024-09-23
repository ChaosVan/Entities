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

            EntityManager.RemoveComponentData<CopyInitialTransformFromGameObject>(entity, CommandBuffer);

            if (EntityManager.TryGetComponentData<Position>(entity, out var position))
                position.Value = component1.position;

            if (EntityManager.TryGetComponentData<Rotation>(entity, out var rotation))
                rotation.Value = component1.rotation;
                
            if (EntityManager.TryGetComponentData<Scale>(entity, out var scale))
                scale.Value = component1.lossyScale;
        }
    }
}
