using UnityEngine;

namespace Assets.Scripts
{
    public abstract class Summonable : MonoBehaviour
    {
        [field: SerializeField] public int ObjectId { get; private set; }

        public abstract void Initialize();
    }
}