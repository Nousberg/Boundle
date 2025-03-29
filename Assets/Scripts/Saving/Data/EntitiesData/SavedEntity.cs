using Assets.Scripts.Entities;
using Assets.Scripts.Network;
using Assets.Scripts.Saving.Data;
using Assets.Scripts.Saving.Data.InventoryData;
using Assets.Scripts.Saving.Data.LiquidsData;
using Assets.Scripts.Saving.EffectsData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Saving
{
    [Serializable]
    public class SavedEntity : SavedObject
    {
        public float health;
        public bool invulnerable;
        public bool flying;
        public int id;
        public EntityNetworkData.Rights rights;

        public List<DamageData.DamageType> damageSensors = new List<DamageData.DamageType>();
        public List<SavedEffect> effects = new List<SavedEffect>();

        public SavedLiquidContainer liquids;
        public SavedInventory inventory;
        public SavedPhysicsProperties physicsProperties;

        public SavedEntity(float health, bool invulnerable, bool flying, int id, int networkId, string ownerUUID, EntityNetworkData.Rights rights, SavedLiquidContainer liquids, SavedInventory inventory, SavedPhysicsProperties physicsProperties, List<DamageData.DamageType> damageSensors, List<SavedEffect> effects, SavedRigidBody rigidbody, SavedTransform transform) : base(transform, rigidbody, ownerUUID, networkId)
        {
            this.flying = flying;
            this.health = health;
            this.invulnerable = invulnerable;
            this.id = id;
            this.damageSensors = damageSensors;
            this.effects = effects;
            this.rights = rights;
            this.liquids = liquids;
            this.inventory = inventory;
            this.physicsProperties = physicsProperties;
        }
    }
}