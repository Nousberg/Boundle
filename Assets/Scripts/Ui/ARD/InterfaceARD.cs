
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.ARD
{
    public class InterfaceARD : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI AmmoText { get; private set; }
        [field: SerializeField] public Button Fire { get; private set; }
        [field: SerializeField] public GameObject TargetsParent { get; private set; }
    }
}
