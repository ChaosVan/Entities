using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Samples.Entities.Spawner
{
	public class CubeSpawnerComponent : ComponentWrapper<CubeSpawner>
	{
		private readonly Queue<GameObject> pooled = new Queue<GameObject>();
		public GameObject prefab;
		public int count;
		public float range;
		public float interval;

		public override void Start()
		{
			base.Start();
			component.count = count;
			component.range = range;

			component.Spawn = Spawn;
			component.Recycle = Recycle;
		}

		private GameObject Spawn(Vector3 position, Quaternion rotation)
		{
			GameObject go;
			if (pooled.Count > 0)
			{
				go = pooled.Dequeue();
				go.transform.SetPositionAndRotation(position, rotation);
			}
			else
				go = GameObject.Instantiate(prefab, position, rotation, transform);
			go.SetActive(true);
			return go;
		}

		private void Recycle(GameObject go)
		{
			go.SetActive(false);
			pooled.Enqueue(go);
		}
	}

	public class CubeTarget : IComponentData { }

	public class CubeSpawner : IComponentData
	{
		public int count;
		public float range;

		public Func<Vector3, Quaternion, GameObject> Spawn;
		public Action<GameObject> Recycle;
	}

	[UpdateInGroup(typeof(LateSimulationSystemGroup))]
	public class CubeSpawnerSystem : SystemBase<CubeSpawner>
	{
		private EntityQuery query = new EntityQuery(typeof(CubeTarget));

		protected override void OnStartRunning()
		{
			base.OnStartRunning();

			CommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
		}

		protected override void OnUpdate(int index, Entity entity, CubeSpawner component1)
		{
			int spawnCount = component1.count;
			if (EntityManager.TryGetEntities(query, out var list))
			{
				spawnCount -= list.Count;
			}

			for (int i = 0; i < spawnCount; i++)
			{
				var translation = UnityEngine.Random.insideUnitSphere * component1.range;
				var newEntity = EntityManager.Create(component1.Spawn(translation, Quaternion.identity), query, CommandBuffer);
				var lifeTime = newEntity.GetOrAddComponentData<LifeTime>(CommandBuffer);
				lifeTime.Value = UnityEngine.Random.Range(3f, 10f);
				lifeTime.OnDestroy = component1.Recycle;
			}
		}
	}
}
