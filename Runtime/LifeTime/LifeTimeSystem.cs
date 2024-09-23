using UnityEngine;
using UnityEngine.Scripting;

namespace Entities
{
    public class LifeTime : IComponentData
    {
        public float Value;
    }

    [Preserve]
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    public class LifeTimeSystem : SystemBase<LifeTime>
    {
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate(int index, Entity entity, LifeTime component)
        {
            component.Value -= Time.DeltaTime;
            if (component.Value <= 0)
            {
                EntityManager.DestroyEntity(entity, out var gameObject, CommandBuffer);
                if (gameObject != null)
                    Object.Destroy(gameObject);
            }
        }
    }
}
