namespace Entities
{
    public class LifeTimeComponent : ComponentWrapper<LifeTime>
    {
        public float lifeTime;
        public override void Start()
        {
            base.Start();
            component.Value = lifeTime;
        }
    }
}
