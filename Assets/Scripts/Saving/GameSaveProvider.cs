using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Effects;
using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.ItemTypes;
using Mirror.BouncyCastle.Asn1.X509.Qualified;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Saving
{
    public class GameSaveProvider : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown saveMenu;

        private const string SAVE_PATH = "Saves/";

        private void Start()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (var file in Directory.GetFiles(SAVE_PATH, "*.json", SearchOption.TopDirectoryOnly))
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                options.Add(new TMP_Dropdown.OptionData(fileName));
                SavesData.Saves.Add(fileName);
            }
            saveMenu.options = options;
        }

        public void SaveGame(string name)
        {
            List<EntityData> entities = new List<EntityData>();

            foreach (var e in FindObjectsOfType<Entity>())
            {
                List<EffectData> effects = new List<EffectData>();
                foreach (var effect in e.GetComponent<EffectContainer>().Effects)
                    effects.Add(new EffectData(effect.Duration, effect.Amplifier, effect.Infinite,nameof(effect)));

                InventoryManager inventoryManager = e.GetComponent<InventoryManager>();
                List<ItemDataContainer> items = new List<ItemDataContainer>();
                foreach (var item in inventoryManager.GetAllItems())
                {
                    if (item is WeaponItem weapon)
                    {
                        items.Add(new WeaponDataContainer(weapon.data.Id, item.quantity, weapon.overheat, weapon.ammo, weapon.maxAmmo));
                    }
                    else
                    {
                        items.Add(new ItemDataContainer(item.data.Id, item.quantity));
                    }
                }
                entities.Add(new EntityData(e.Health, e.Blood, e.BaseHealth, e.Invulnerable, e.DamageSensors, effects, new InventoryData(inventoryManager.CurrentItem, items)));
            }

            if (JsonSaver.Save(new GameDataContainer(entities), name) && !SavesData.Saves.Contains(name))
                saveMenu.options.Add(new TMP_Dropdown.OptionData(name));
        }
    }
}