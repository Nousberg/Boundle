using Assets.Scripts.Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Inventory
{
    public class InventoryDataVisualizer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryDataController inventory;
        [SerializeField] private Transform itemIconsParent;

        [Header("Properties")]
        [Range(0f, 1f)][SerializeField] private float selectedItemOpacity;

        private List<GameObject> icons = new List<GameObject>();

        private void Start()
        {
            inventory.OnItemAdded += UpdateShowedIcons;
            inventory.OnItemRemoved += UpdateShowedIcons;
            inventory.OnItemSwitched += UpdateShowedIcons;
        }
        private void UpdateShowedIcons()
        {
            foreach (var icon in icons)
                Destroy(icon);

            icons.Clear();

            foreach (var item in inventory.GetItems)
            {
                GameObject icon = new GameObject(item.data.name);
                Image img = icon.AddComponent<Image>();
                img.sprite = item.data.Icon;
                img.preserveAspect = true;

                if (inventory.GetItems.IndexOf(item) == inventory.CurrentItemIndex)
                    img.color = new Color(img.color.r, img.color.g, img.color.b, selectedItemOpacity);

                icon.AddComponent<Button>().onClick.AddListener(() => inventory.SwitchItem(inventory.GetItems.IndexOf(item)));
                icon.transform.SetParent(itemIconsParent, false);

                icons.Add(icon);

            }
        }
    }
}