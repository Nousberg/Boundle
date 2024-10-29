using UnityEngine;

namespace Assets.Scripts.Movement.Scriptables
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Movement/Data")]
    public class MovementData : ScriptableObject
    {
        [field: SerializeField] public float WalkSpeed { get; private set; }
        [field: SerializeField] public float FlySpeed { get; private set; }
        [field: SerializeField] public float RunSpeedBoost { get; private set; }
        [field: SerializeField] public float JumpPower { get; private set; }
    }
}
