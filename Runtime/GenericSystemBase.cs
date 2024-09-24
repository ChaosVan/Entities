using UnityEngine;

namespace Entities
{
	public abstract class SystemBase<T1> : SystemBase where T1 : IComponentData
	{
		protected override void OnCreate()
		{
			OverwriteEntityQuery(ComponentType.ReadWrite<T1>());
		}
		protected sealed override void Eject(int index)
		{
			OnEject(Entities[index], (T1)GetComponentData(index, 0));
		}
		protected sealed override void OnUpdate()
		{
			for (int i = 0; i < Entities.Count; ++i)
			{
				OnUpdate(i, Entities[i], (T1)GetComponentData(i, 0));
			}
		}
		protected abstract void OnUpdate(int index, Entity entity, T1 component1);
		protected virtual void OnEject(Entity entity, T1 component1) { }
	}
	public abstract class SystemBase<T1, T2> : SystemBase where T1 : IComponentData where T2 : IComponentData
	{
		protected override void OnCreate()
		{
			OverwriteEntityQuery(ComponentType.ReadWrite<T1>(), ComponentType.ReadWrite<T2>());
		}
		protected sealed override void Eject(int index)
		{
			OnEject(Entities[index], (T1)GetComponentData(index, 0), (T2)GetComponentData(index, 1));
		}
		protected sealed override void OnUpdate()
		{
			for (int i = 0; i < Entities.Count; ++i)
			{
				OnUpdate(i, Entities[i], (T1)GetComponentData(i, 0), (T2)GetComponentData(i, 1));
			}
		}
		protected abstract void OnUpdate(int index, Entity entity, T1 component1, T2 component2);
		protected virtual void OnEject(Entity entity, T1 component1, T2 component2) { }
	}
	public abstract class SystemBase<T1, T2, T3> : SystemBase where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData
	{
		protected override void OnCreate()
		{
			OverwriteEntityQuery(ComponentType.ReadWrite<T1>(), ComponentType.ReadWrite<T2>(), ComponentType.ReadWrite<T3>());
		}
		protected sealed override void Eject(int index)
		{
			OnEject(Entities[index], (T1)GetComponentData(index, 0), (T2)GetComponentData(index, 1), (T3)GetComponentData(index, 2));
		}
		protected sealed override void OnUpdate()
		{
			for (int i = 0; i < Entities.Count; ++i)
			{
				OnUpdate(i, Entities[i], (T1)GetComponentData(i, 0), (T2)GetComponentData(i, 1), (T3)GetComponentData(i, 2));
			}
		}
		protected abstract void OnUpdate(int index, Entity entity, T1 component1, T2 component2, T3 component3);
		protected virtual void OnEject(Entity entity, T1 component1, T2 component2, T3 component3) { }
	}
	public abstract class SystemBase<T1, T2, T3, T4> : SystemBase where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData
	{
		protected override void OnCreate()
		{
			OverwriteEntityQuery(ComponentType.ReadWrite<T1>(), ComponentType.ReadWrite<T2>(), ComponentType.ReadWrite<T3>(), ComponentType.ReadWrite<T4>());
		}
		protected sealed override void Eject(int index)
		{
			OnEject(Entities[index], (T1)GetComponentData(index, 0), (T2)GetComponentData(index, 1), (T3)GetComponentData(index, 2), (T4)GetComponentData(index, 3));
		}
		protected sealed override void OnUpdate()
		{
			for (int i = 0; i < Entities.Count; ++i)
			{
				OnUpdate(i, Entities[i], (T1)GetComponentData(i, 0), (T2)GetComponentData(i, 1), (T3)GetComponentData(i, 2), (T4)GetComponentData(i, 3));
			}
		}
		protected abstract void OnUpdate(int index, Entity entity, T1 component1, T2 component2, T3 component3, T4 component4);
		protected virtual void OnEject(Entity entity, T1 component1, T2 component2, T3 component3, T4 component4) { }
	}
	public abstract class HybridSystemBase<T1, T2> : HybridSystemBase<T1> where T1 : Component where T2 : IComponentData
	{
		protected override void OnCreate()
		{
			OverwriteEntityQuery(ComponentType.ReadWrite<T2>());
		}
		protected sealed override void Eject(int index)
		{
			OnEject(Entities[index], ComponentObjects[index], (T2)GetComponentData(index, 0));
		}
		protected sealed override void OnUpdate()
		{
			for (int i = 0; i < Entities.Count; ++i)
			{
				OnUpdate(i, Entities[i], ComponentObjects[i], (T2)GetComponentData(i, 0));
			}
		}
		protected abstract void OnUpdate(int index, Entity entity, T1 component1, T2 component2);
		protected virtual void OnEject(Entity entity, T1 component1, T2 component2) { }
	}
	public abstract class HybridSystemBase<T1, T2, T3> : HybridSystemBase<T1> where T1 : Component where T2 : IComponentData where T3 : IComponentData
	{
		protected override void OnCreate()
		{
			OverwriteEntityQuery(ComponentType.ReadWrite<T2>(), ComponentType.ReadWrite<T3>());
		}
		protected sealed override void Eject(int index)
		{
			OnEject(Entities[index], ComponentObjects[index], (T2)GetComponentData(index, 0), (T3)GetComponentData(index, 1));
		}
		protected sealed override void OnUpdate()
		{
			for (int i = 0; i < Entities.Count; ++i)
			{
				OnUpdate(i, Entities[i], ComponentObjects[i], (T2)GetComponentData(i, 0), (T3)GetComponentData(i, 1));
			}
		}
		protected abstract void OnUpdate(int index, Entity entity, T1 component1, T2 component2, T3 component3);
		protected virtual void OnEject(Entity entity, T1 component1, T2 component2, T3 component3) { }
	}
	public abstract class HybridSystemBase<T1, T2, T3, T4> : HybridSystemBase<T1> where T1 : Component where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData
	{
		protected override void OnCreate()
		{
			OverwriteEntityQuery(ComponentType.ReadWrite<T2>(), ComponentType.ReadWrite<T3>(), ComponentType.ReadWrite<T4>());
		}
		protected sealed override void Eject(int index)
		{
			OnEject(Entities[index], ComponentObjects[index], (T2)GetComponentData(index, 0), (T3)GetComponentData(index, 1), (T4)GetComponentData(index, 2));
		}
		protected sealed override void OnUpdate()
		{
			for (int i = 0; i < Entities.Count; ++i)
			{
				OnUpdate(i, Entities[i], ComponentObjects[i], (T2)GetComponentData(i, 0), (T3)GetComponentData(i, 1), (T4)GetComponentData(i, 2));
			}
		}
		protected abstract void OnUpdate(int index, Entity entity, T1 component1, T2 component2, T3 component3, T4 component4);
		protected virtual void OnEject(Entity entity, T1 component1, T2 component2, T3 component3, T4 component4) { }
	}
}
