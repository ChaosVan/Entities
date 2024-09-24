using Entities;
using Unity.Mathematics;

namespace Samples.Entities.RollSystem
{
	public class RollSpeedComponent : ComponentWrapper<RollSpeed>
	{
		public float3 value = 0;
		public override void Start()
		{
			base.Start();
			component.value = value;
		}
	}

	public class RollSpeed : IComponentData
	{
		public float3 value;
	}
}
