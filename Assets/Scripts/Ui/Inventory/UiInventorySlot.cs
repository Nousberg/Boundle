using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Inventory
{
    public class UiInventorySlot : MonoBehaviour
    {
        [field: Header("References")]
        [field: SerializeField] public Image Icon { get; private set; }
        [field: SerializeField] public TextMeshProUGUI QuantityText { get; private set; }
    }
}