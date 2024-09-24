using Entities;
using UnityEngine;

namespace Samples.Entities.Spawner
{
	public class Muzzle : IComponentData
	{
		public Transform Point;
	}

	public class MuzzleComponent : ComponentWrapper<Muzzle>
	{
		public Transform Point;

        public override void Start()
        {
            base.Start();

            component.Point = Point;
        }
	}
}
