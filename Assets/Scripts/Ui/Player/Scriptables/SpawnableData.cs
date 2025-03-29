using UnityEngine;

namespace Assets.Scripts.Ui.Player.Scriptables
{
    [CreateAssetMenu(fileName = "Spawnable", menuName = "ScriptableObjects/Summoning/Spawnable")]
    public class SpawnableData : ScriptableObject
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public ObjectCategory Category { get; private set; }

        public enum ObjectCategory : byte
        {
            None,
            Living,
            Rifle,
            Syringe,
            Prop
        }
    }
}
