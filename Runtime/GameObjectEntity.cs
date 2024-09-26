using UnityEngine;

namespace Entities
{
    public sealed class GameObjectEntity : MonoBehaviour
    {
        private static EntityQuery fromArchetype, toArchetype;

        [Tooltip("If true, the Entity's Matrix will be copied from the GameObject, otherwise the Entity's Matrix will be copied to the GameObject")]
        [SerializeField]
        private bool copyFromGameObject;
        [Tooltip("If true, the Entity will be destroyed when the GameObject is destroyed")]
        [SerializeField]
        private bool autoDestroy;
        private Entity entity;
        public Entity Entity => entity;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            fromArchetype = new(
                    typeof(LocalToWorld),
                    typeof(CopyInitialTransformFromGameObject),
                    typeof(CopyTransformFromGameObject));
                

            toArchetype = new(
                    typeof(LocalToWorld),
                    typeof(CopyInitialTransformFromGameObject),
                    typeof(CopyTransformToGameObject),
                    typeof(Position),
                    typeof(Rotation),
                    typeof(Scale));
        }

        private void Start()
        {
            if (copyFromGameObject)
                entity = EntityManager.Create(gameObject, fromArchetype);
            else
                entity = EntityManager.Create(gameObject, toArchetype);
        }

        private void OnDestroy()
        {
            if (autoDestroy)
                EntityManager.DestroyEntity(entity, out _);
            else
                EntityManager.UnbindGameObject(entity, out _);
        }
    }
}
