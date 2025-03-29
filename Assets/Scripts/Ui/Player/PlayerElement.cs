using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Player
{
    public class PlayerElement : MonoBehaviour
    {
        [field: SerializeField] public Image Status { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Name { get; private set; }
    }
}
