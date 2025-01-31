﻿using Assets.Scripts.Entities;
using System.Collections.Generic;

namespace Assets.Scripts.Saving
{
    public class EntityData
    {
        public float currentHealth, currentBlood, baseHealth;
        public bool invulnerable;
        public List<DamageData.DamageType> damageSensors = new List<DamageData.DamageType>();
        public List<EffectData> effects = new List<EffectData>();

        public EntityData(float currentHealth, float currentBlood, float baseHealth, bool invulnerable, List<DamageData.DamageType> damageSensors, List<EffectData> effects)
        {
            this.currentBlood = currentBlood;
            this.currentHealth = currentHealth;
            this.effects = effects;
            this.baseHealth = baseHealth;
            this.invulnerable = invulnerable;
            this.damageSensors = damageSensors;
        }
    }
}
