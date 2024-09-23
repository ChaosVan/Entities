/*
 * COPYRIGHT © 2024 CHANGYOU.COM LIMITED. ALL RIGHTS RESERVED.
 * 
 * FILENAME:    RollSystem.cs
 * TIME:        2024年9月13日 10:08:29
 * AUTHOR:      赵朝凡
 * CONTACT:     zhaochaofan@cyou-inc.com
 * DESCRIPTION: RollSystem
 */

using Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Samples.Entities.RollSystem
{
	[Preserve]
	public class RollSystem : SystemBase<Rotation, RollSpeed>
	{
		protected override void OnUpdate(int index, Entity entity, Rotation component1, RollSpeed component2)
		{
			component1.Value = math.mul(component1.Value, quaternion.Euler(math.radians(component2.value) * Time.DeltaTime));
		}
	}
}
