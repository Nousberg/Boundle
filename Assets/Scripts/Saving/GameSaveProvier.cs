using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Effects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Saving
{
    public class GameSaveProvier : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown saveMenu;

        public void SaveGame(string name)
        {
            List<EntityData> entities = new List<EntityData>();

            foreach (var e in FindObjectsOfType<Entity>())
            {
               // EntityInventory inv = e.GetComponent<EntityInventory>();
                List<EffectData> effects = new List<EffectData>();
                foreach (var ef in e.Effects)
                {
                    effects.Add(new EffectData(ef.Duration, ef.Amplifier, ef.GetType().Name));
                }
               // entities.Add(new EntityData(e.Health, e.BaseHealth, e.Invulnerable, e.DamageSensors, effects, new InventoryData(inv.Items, inv.currentItem)));
            }

            if (JsonSaver.Save(new GameDataContainer(entities), name) && !JsonSaver.Saves.Contains(name))
                saveMenu.options.Add(new TMP_Dropdown.OptionData(name));
        }
    }
}