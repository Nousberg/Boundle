using TMPro;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class PlayerInfoUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerName;

        private void Start() => playerName.text = PlayerPrefs.GetString("playerName");
    }
}
