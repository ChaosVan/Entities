using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Samples.Entities.Spawner
{
	public class BarrelAttacker : ComponentWrapper<RigidbodyAttacker>
	{
		private readonly Queue<GameObject> pooled = new Queue<GameObject>();
		public GameObject Bullet;
		public float power;
		public float rate;
		public float timer;
		public float life;

		public override void Start()
		{
			base.Start();
			component.power = power;
			component.rate = rate;
			component.timer = timer;
			component.life = life;

			component.Spawn = Spawn;
			component.Recycle = Recycle;
		}

		private Rigidbody Spawn(Vector3 position, Quaternion rotation)
		{
			GameObject go;
			if (pooled.Count > 0)
			{
				go = pooled.Dequeue();
				go.transform.SetPositionAndRotation(position, rotation);
			}
			else
				go = GameObject.Instantiate(Bullet, position, rotation, transform);
			go.SetActive(true);
			return go.GetComponent<Rigidbody>();
		}

		private void Recycle(GameObject go)
		{
			var body = go.GetComponent<Rigidbody>();
			body.velocity = Vector3.zero;
			body.angularVelocity = Vector3.zero;
			go.SetActive(false);
			pooled.Enqueue(go);
		}
	}

	public class RigidbodyAttacker : IComponentData
	{
		public float power;
		public float rate;
		public float timer;
		public float life;

		public Func<Vector3, Quaternion, Rigidbody> Spawn;
		public Action<GameObject> Recycle;
	}

	public class BarrelAttackerSystem : SystemBase<RigidbodyAttacker, Muzzle>
	{
		protected override void OnStartRunning()
		{
			base.OnStartRunning();

			CommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
		}

		protected override void OnUpdate(int index, Entity entity, RigidbodyAttacker component1, Muzzle component2)
		{
			if (component1.timer <= 0)
			{
				Transform p = component2.Point;
				var body = component1.Spawn(p.position, p.rotation);
				body.AddForce(p.forward * component1.power, ForceMode.Impulse);
				var bullet = EntityManager.Create(body.gameObject, CommandBuffer);
				var lifeTime = bullet.GetOrAddComponentData<LifeTime>(CommandBuffer);
				lifeTime.Value = component1.life;
				lifeTime.OnDestroy += component1.Recycle;

				component1.timer += 1f / component1.rate;
			}
			else
			{
				component1.timer -= Time.DeltaTime;
			}
		}
	}
}
