using UnityEngine;

namespace Assets.Scripts.Ui.Player.Scriptables
{
    [CreateAssetMenu(fileName = "Status", menuName = "ScriptableObjects/Multiplayer/Status")]
    public class StatusData : ScriptableObject
    {
        [field: SerializeField] public int Identifier { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}
