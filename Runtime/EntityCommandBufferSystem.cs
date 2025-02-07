using System.Collections.Generic;

namespace Entities
{
    public class EntityCommandBufferSystem : ComponentSystem
    {
        List<EntityCommandBuffer> m_PendingBuffers;
        internal List<EntityCommandBuffer> PendingBuffers => m_PendingBuffers;

        public EntityCommandBuffer CreateCommandBuffer()
        {
            var cmds = ReferencePool.SpawnInstance<EntityCommandBuffer>();
            var world = World.Unmanaged;
            cmds.SystemID = world.ExecutingSystem.m_SystemId;
            cmds.OriginSystemHandle = world.ExecutingSystem;

            m_PendingBuffers.Add(cmds);

            return cmds;
        }

        /// <summary>
        /// Initializes this command buffer system.
        /// </summary>
        /// <remarks>If you override this method, you should call `base.OnCreate()` to retain the default
        /// initialization logic.</remarks>
        protected override void OnCreate()
        {
            base.OnCreate();
            m_PendingBuffers = new List<EntityCommandBuffer>();
        }

        /// <summary>
        /// Destroys this system, executing any pending command buffers first.
        /// </summary>
        /// <remarks>If you override this method, you should call `base.OnDestroy()` to retain the default
        /// destruction logic.</remarks>
        protected override void OnDestroy()
        {
            FlushPendingBuffers(false);
            m_PendingBuffers.Clear();

            base.OnDestroy();
        }

        /// <summary>
        /// Executes the command buffers in this system in the order they were created.
        /// </summary>
        /// <remarks>If you override this method, you should call `base.OnUpdate()` to retain the default
        /// update logic.</remarks>
        protected override void OnUpdate()
        {
            FlushPendingBuffers(true);
            m_PendingBuffers.Clear();
        }

        internal void FlushPendingBuffers(bool playBack)
        {
            if (m_PendingBuffers.Count > 0)
            {
                var entities = EntityManager.BeginStructual(0);
                foreach (var buffer in m_PendingBuffers)
                {
                    if (playBack)
                    {
                        var system = GetSystemFromSystemID(World, buffer.SystemID);
                        buffer.Playback(ref entities);
                    }

                    ReferencePool.RecycleInstance(buffer);
                }
                EntityManager.EndStructual(entities);
            }

        }
    }
}
