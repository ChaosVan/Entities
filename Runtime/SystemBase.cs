#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;

namespace Entities
{
    public abstract class SystemBase : ComponentSystemBase
    {
#if ODIN_INSPECTOR
        [ShowIf("showOdinInfo"), ShowInInspector, ListDrawerSettings(IsReadOnly = true)]
#endif
        internal readonly List<Entity> Entities = new List<Entity>();
        internal readonly List<EntityData> Components = new List<EntityData>();
        protected EntityCommandBufferSystem CommandBufferSystem;
        protected EntityCommandBuffer CommandBuffer;
        internal EntityQuery query;

        internal virtual void InjectEntity(Entity entity)
        {
            bool match = EntityManager.Match(entity, query, out var data);
            if (Entities.Contains(entity))
            {
                if (!match)
                {
                    var i = Entities.IndexOf(entity);
                    Eject(i);
                    Components.RemoveAt(i);
                    Entities.RemoveAt(i);
                }
            }
            else if (match)
            {
                Components.Add(data);
                Entities.Add(entity);
            }
        }

        protected void OverwriteEntityQuery(params ComponentType[] types)
        {
            query = new EntityQuery(types);
        }

        protected IComponentData GetComponentData(int index, int queryIndex)
        {
            if (Components[index].InternalGetComponentData(query[queryIndex], out var componentData))
                return componentData;
            
            return null;
        }

        protected abstract void Eject(int index);

        protected virtual void PreUpdate()
        {
            if (CommandBufferSystem != null)
                CommandBuffer = CommandBufferSystem.CreateCommandBuffer();
        }

        protected abstract void OnUpdate();

        protected virtual void PostUpdate()
        {
            CommandBuffer = null;
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            CommandBufferSystem = null;
            Entities.Clear();
            Components.Clear();
        }

        public sealed override bool ShouldRunSystem()
        {
            if (Entities.Count == 0)
                return false;

            return base.ShouldRunSystem();
        }

        public sealed override void Update()
        {
            CheckedState();
            if (Enabled && ShouldRunSystem())
            {
                if (!m_State.PreviouslyEnabled)
                {
                    m_State.PreviouslyEnabled = true;
                    OnStartRunning();
                }

                var world = World.Unmanaged;
                var oldExecutingSystem = world.ExecutingSystem;
                world.ExecutingSystem = m_State.m_Handle;

                try
                {
                    PreUpdate();
                    OnUpdate();
                }
                finally
                {
                    PostUpdate();
                    world.ExecutingSystem = oldExecutingSystem;
                }
            }
            else if (m_State.PreviouslyEnabled)
            {
                m_State.PreviouslyEnabled = false;
                OnStopRunningInternal();
            }
        }

        internal sealed override void OnBeforeDestroyInternal()
        {
            base.OnBeforeDestroyInternal();
        }

        internal sealed override void OnBeforeCreateInternal(World world)
        {
            base.OnBeforeCreateInternal(world);
        }

        internal sealed override void OnStopRunningInternal()
        {
            base.OnStopRunningInternal();
        }
    }
}
