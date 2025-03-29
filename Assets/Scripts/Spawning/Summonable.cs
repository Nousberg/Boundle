using UnityEngine;

namespace Assets.Scripts
{
    public abstract class Summonable : MonoBehaviour
    {
        [field: SerializeField] public int ObjectId { get; private set; }
        [field: SerializeField] public Vector3 SpawnOffset { get; private set; }

        public abstract void Initialize(GameObject metaObject);
    }
}