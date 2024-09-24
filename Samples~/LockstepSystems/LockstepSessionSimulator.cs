using Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Samples.Entities.LockstepSystems
{
	public class LockstepSessionSimulator : SingletonBehaviour<LockstepSessionSimulator>
	{
		private LockstepRateManager lockstepRateManager;

#if ODIN_INSPECTOR
		[ShowIf("showOdinInfo"), ShowInInspector]
		public int CurrentFrameIndex => lockstepRateManager != null ? lockstepRateManager.CurrentFrameIndex : 0;
#endif

		protected override void OnInitialized()
		{
			base.OnInitialized();

			updateMode = UpdateMode.FIXED_UPDATE;

			var lockstepSystemGroup = World.DefaultGameObjectInjectionWorld.GetExistingSystem<LockstepSystemGroup>();
			lockstepRateManager = lockstepSystemGroup.RateManager as LockstepRateManager;
			lockstepRateManager.Reset(Time.fixedDeltaTime);
		}

		protected override void OnUpdate(float delta)
		{
			lockstepRateManager.WantedFrameIndex++;
		}

		private void Start()
		{
			var commandBuffer = EntityManager.CreateBeginCommandBuffer();
			var entity = EntityManager.Create(transform.Find("Cube").gameObject, commandBuffer);
			entity.GetOrAddComponentData<RollSpeed>(commandBuffer).value = new Vector3(0, 90, 0);
		}

		public void PauseOrResume()
		{
			this.enabled = !this.enabled;
		}
	}
}
