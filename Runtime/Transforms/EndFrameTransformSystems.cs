using UnityEngine.Scripting;

namespace Entities
{
    [Preserve]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(LateSimulationSystemGroup))]
    public class TransformSystemGroup : ComponentSystemGroup
    {
    }
}
