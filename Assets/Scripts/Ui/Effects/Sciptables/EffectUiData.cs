using UnityEngine;

namespace Assets.Scripts.Ui.Effects.Sciptables
{

    [CreateAssetMenu(fileName = "EffectUiData", menuName = "ScriptableObjects/Ui/EffectUiData")]
    public class EffectUiData : ScriptableObject
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}
