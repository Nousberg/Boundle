using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Effects;
using System.Collections.Generic;

namespace Assets.Scripts.Saving
{
    public class EntityData
    {
        public float currentHealth, currentBlood, baseHealth;
        public bool invulnerable;
        public List<DamageType> damageSensors = new List<DamageType>();
        public List<EffectData> effects = new List<EffectData>();
        public InventoryData inventory;

        public EntityData(float currentHealth, float currentBlood, float baseHealth, bool invulnerable, List<DamageType> damageSensors, List<EffectData> effects, InventoryData inventory)
        {
            this.currentBlood = currentBlood;
            this.currentHealth = currentHealth;
            this.effects = effects;
            this.baseHealth = baseHealth;
            this.invulnerable = invulnerable;
            this.damageSensors = damageSensors;
            this.inventory = inventory;
        }
    }
}
