using Assets.Scripts.Inventory;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Ui.Inventory;

namespace Assets.Scripts.Ui
{
    [RequireComponent(typeof(InventoryManager))]
    public class InventoryUiManager : MonoBehaviour
    {
        [SerializeField] private List<UiInventorySlot> Slots = new List<UiInventorySlot>();

        private InventoryManager inventory => GetComponent<InventoryManager>();

        private void Start()
        {
            inventory.OnInventoryChanged += UpdateUi;
        }

        public void UpdateUi(DefaultItem item, int index)
        {
            Slots[index].Icon.sprite = item.data.Icon;
        }
    }
}