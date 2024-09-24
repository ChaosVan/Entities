using Entities;
using UnityEngine;

namespace Samples.Entities.Spawner
{
	public class CubeSpawnerComponent : ComponentWrapper<CubeSpawner>
	{
		public GameObject prefab;
		public int count;
		public float range;

		public override void Start()
		{
			base.Start();
			component.root = transform;
			component.prefab = prefab;
			component.count = count;
			component.range = range;
		}
	}

	public class CubeTarget : IComponentData { }

	public class CubeSpawner : IComponentData
	{
		public Transform root;
		public GameObject prefab;
		public int count;
		public float range;
	}

	[UpdateInGroup(typeof(LateSimulationSystemGroup))]
	public class CubeSpawnerSystem : SystemBase<CubeSpawner>
	{
		private EntityArchetype archetype = new EntityArchetype(typeof(CubeTarget));

		protected override void OnStartRunning()
		{
			base.OnStartRunning();

			CommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
		}

		protected override void OnUpdate(int index, Entity entity, CubeSpawner component1)
		{
			int spawnCount = component1.count;
			if (EntityManager.TryGetEntities(new EntityQuery(typeof(CubeTarget)), out var list))
			{
				spawnCount -= list.Count;
			}

			for (int i = 0; i < spawnCount; i++)
			{
				var translation = Random.insideUnitSphere * component1.range;
				var gameObject = Object.Instantiate(component1.prefab, translation, Quaternion.identity, component1.root);
				var newEntity = EntityManager.Create(gameObject, archetype, CommandBuffer);
				newEntity.GetOrAddComponentData<LifeTime>(CommandBuffer).Value = Random.Range(3f, 10f);
				gameObject.SetActive(true);
			}
		}
	}
}
