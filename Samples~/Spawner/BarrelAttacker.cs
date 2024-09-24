using Entities;
using UnityEngine;

namespace Samples.Entities.Spawner
{
	public class Attacker : IComponentData
	{
		public GameObject Bullet;
		public float power;

		public float rate;
		public float timer;
		public float life;

	}

	public class BarrelAttacker : ComponentWrapper<Attacker>
	{
	    public GameObject Bullet;
		public float power;
		public float rate;
		public float timer;
		public float life;

        public override void Start()
        {
            base.Start();
			component.Bullet = Bullet;
			component.power = power;
			component.rate = rate;
			component.timer = timer;
			component.life = life;
        }
    }

    public class BarrelAttackerSystem : SystemBase<Attacker, Muzzle>
    {
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

			CommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate(int index, Entity entity, Attacker component1, Muzzle component2)
        {
            if (component1.timer <= 0)
            {
                Transform p = component2.Point;
				var gameObject = GameObject.Instantiate(component1.Bullet, p.position, p.rotation);
				gameObject.SetActive(true);
				gameObject.GetComponent<UnityEngine.Rigidbody>().AddForce(p.forward * component1.power, ForceMode.Impulse);
				var bullet = EntityManager.Create(gameObject, CommandBuffer);
				bullet.GetOrAddComponentData<LifeTime>(CommandBuffer).Value = component1.life;

                component1.timer += 1f / component1.rate;
            }
            else
            {
                component1.timer -= Time.DeltaTime;
            }
        }
    }
}
