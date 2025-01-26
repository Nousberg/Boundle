using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Multiplayer
{
    public class SceneElement : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Name { get; private set; }
        [field: SerializeField] public Image Icon { get; private set; }
        [field: SerializeField] public Button SButton { get; private set; }
    }
}
