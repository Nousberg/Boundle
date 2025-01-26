using UnityEngine;
using Assets.Scripts.Spawning;

namespace Assets.Scripts
{
    public abstract class Summonable : MonoBehaviour
    {
        [field: SerializeField] public int ObjectId { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Summonables.ObjectCategory Category { get; private set; }

        public abstract void Initialize(GameObject metaObject);
    }
}