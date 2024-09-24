using Entities;

namespace Samples.Entities.LockstepSystems
{
	public class LockstepSystemGroup : ComponentSystemGroup
	{
		public LockstepSystemGroup()
		{
			RateManager = new LockstepRateManager();
		}
	}

	[UpdateInGroup(typeof(LockstepSystemGroup), OrderFirst = true)]
	public class BeginLockstepEntityCommandBufferSystem : EntityCommandBufferSystem { }

	[UpdateInGroup(typeof(LockstepSystemGroup), OrderLast = true)]
	public class EndLockstepEntityCommandBufferSystem : EntityCommandBufferSystem { }

	public class LockstepRateManager : IRateManager
	{
		private int currentFrameIndex, wantedFrameIndex;

		public int CurrentFrameIndex => currentFrameIndex;
		public int WantedFrameIndex
		{
			get => wantedFrameIndex;
			set => wantedFrameIndex = value;
		}
		public float Timestep { get; set; }
		public float ElapsedTime => Timestep * currentFrameIndex;

		private bool m_DidPushTime;

		public bool ShouldGroupUpdate(ComponentSystemGroup group)
		{
			if (m_DidPushTime)
			{
				group.World.PopTime();
			}

			if (currentFrameIndex < wantedFrameIndex)
			{
				currentFrameIndex++;
				group.World.PushTime(new TimeData(ElapsedTime, Timestep));
				m_DidPushTime = true;
				return true;
			}
			else
			{
				m_DidPushTime = false;
				return false;
			}
		}

		public void Reset(float timestep)
		{
			currentFrameIndex = 0;
			wantedFrameIndex = 0;
			Timestep = timestep;
		}
	}
}
