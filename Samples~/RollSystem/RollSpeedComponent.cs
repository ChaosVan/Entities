/*
 * COPYRIGHT © 2024 CHANGYOU.COM LIMITED. ALL RIGHTS RESERVED.
 * 
 * FILENAME:    RollSpeedComponent.cs
 * TIME:        2024年9月13日 10:04:18
 * AUTHOR:      赵朝凡
 * CONTACT:     zhaochaofan@cyou-inc.com
 * DESCRIPTION: RollSpeedComponent
 */

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
