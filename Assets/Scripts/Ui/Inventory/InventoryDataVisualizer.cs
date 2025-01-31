﻿using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.DynamicData;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Inventory
{
    [RequireComponent(typeof(InventoryDataController))]
    public class InventoryDataVisualizer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image ammoBackground;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private GameObject ammoContainer;
        [SerializeField] private Transform itemIconsParent;

        [Header("Properties")]
        [Range(0f, 1f)][SerializeField] private float selectedItemOpacity;

        private InventoryDataController inventory => GetComponent<InventoryDataController>();

        private List<GameObject> icons = new List<GameObject>();

        public void Init()
        {
            inventory.OnItemAdded += UpdateShowedIcons;
            inventory.OnItemRemoved += UpdateShowedIcons;
            inventory.OnItemSwitched += UpdateShowedIcons;
        }
        private void ShowAmmo()
        {
            if (inventory.GetItems[inventory.CurrentItemIndex] is DynamicWeaponData weapon)
            {
                ammoText.text = weapon.currentAmmo.ToString() + " / " + weapon.overallAmmo.ToString();
                ammoBackground.fillAmount = (float)weapon.currentAmmo / weapon.overallAmmo;
                ammoContainer.SetActive(true);
            }
            else
                ammoContainer.SetActive(false);
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

                if (ReferenceEquals(inventory.GetItems[inventory.CurrentItemIndex], item))
                    img.color = new Color(img.color.r, img.color.g, img.color.b, selectedItemOpacity);

                icon.AddComponent<Button>().onClick.AddListener(() => inventory.SwitchItem(inventory.GetItems.IndexOf(item)));
                icon.transform.SetParent(itemIconsParent, false);

                icons.Add(icon);

                if (inventory.AllInGameItems.Find(n => n.BaseData.Id == item.data.Id) is WeaponDataController weapon)
                {
                    weapon.OnAmmoChanged -= ShowAmmo;
                    weapon.OnAmmoChanged += ShowAmmo;
                }
            }

            ShowAmmo();
        }
    }
}