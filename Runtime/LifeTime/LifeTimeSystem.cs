using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Entities
{
    public class LifeTime : IComponentData, IDisposable
    {
        public float Value;

        private Action<GameObject> destroy;
        public Action<GameObject> OnDestroy
        {
            get => destroy;
            set => destroy = value;
        }

        public void Dispose()
        {
            destroy = null;
        }

        internal void Destroy(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            if (destroy != null)
                destroy.Invoke(gameObject);
            else
                UnityEngine.Object.Destroy(gameObject);
        }
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
                component.Destroy(gameObject);
            }
        }
    }
}
