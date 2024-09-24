using Entities;
using Unity.Mathematics;

namespace Samples.Entities.LockstepSystems
{
	public class RollSpeed : IComponentData
	{
		public float3 value;
	}

	[UpdateInGroup(typeof(LockstepSystemGroup))]
	public class RollSystem : SystemBase<Rotation, RollSpeed>
	{
		protected override void OnUpdate(int index, Entity entity, Rotation component1, RollSpeed component2)
		{
			component1.Value = math.mul(component1.Value, quaternion.Euler(math.radians(component2.value) * Time.DeltaTime));
		}
	}
}
