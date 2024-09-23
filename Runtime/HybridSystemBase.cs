using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public abstract class HybridSystemBase<T1> : SystemBase where T1 : Component
    {
        internal readonly List<T1> ComponentObjects = new List<T1>();

        internal override void InjectEntity(Entity entity)
        {
            bool match = Match(entity, query, out var data, out T1 component);
            if (Entities.Contains(entity))
            {
                if (!match)
                {
                    var i = Entities.IndexOf(entity);
                    Eject(i);
                    ComponentObjects.RemoveAt(i);
                    Components.RemoveAt(i);
                    Entities.RemoveAt(i);
                }
            }
            else if (match)
            {
                ComponentObjects.Add(component);
                Components.Add(data);
                Entities.Add(entity);
            }
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            ComponentObjects.Clear();
        }

        private bool Match(Entity entity, EntityQuery query, out EntityData data, out T1 component)
        {
            component = null;
            return EntityManager.Match(entity, query, out data) && EntityManager.TryGetComponentObject<T1>(entity, out component);
        }
    }
}
