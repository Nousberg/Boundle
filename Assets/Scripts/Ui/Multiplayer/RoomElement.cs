using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Multiplayer
{
    public class RoomElement : MonoBehaviour
    {
        [field: SerializeField] public Button ToggleButton { get; private set; }
        [field: SerializeField] public TextMeshProUGUI PlayersCount { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Name { get; private set; }
        [field: SerializeField] public Image Icon { get; private set; }
        [field: SerializeField] public GameObject Password { get; private set; }
        [field: SerializeField] public TMP_InputField Input { get; private set; }
    }
}
