using UnityEngine;

namespace Entities
{
    public interface IComponentWrapper
    {
        void Start();
    }

    public class ComponentWrapper<T> : MonoBehaviour, IComponentWrapper where T : IComponentData
    {
        protected T component;
        protected Entity entity;

        public virtual void Start()
        {
            var commandBuffer = EntityManager.CreateBeginCommandBuffer();
            entity = EntityManager.Create(gameObject, commandBuffer);
            component = entity.GetOrAddComponentData<T>(commandBuffer);
        }
    }

    public class ComponentWrapper<T1, T2> : MonoBehaviour, IComponentWrapper where T1 : IComponentData where T2 : IComponentData
    {
        protected T1 component1;
        protected T2 component2;
        protected Entity entity;

        public virtual void Start()
        {
            var commandBuffer = EntityManager.CreateBeginCommandBuffer();
            entity = EntityManager.Create(gameObject, commandBuffer);
            component1 = entity.GetOrAddComponentData<T1>(commandBuffer);
            component2 = entity.GetOrAddComponentData<T2>(commandBuffer);
        }
    }
}
