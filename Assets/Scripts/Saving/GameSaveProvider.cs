using Assets.Scripts.Core.Environment;
using Assets.Scripts.Effects;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Liquids;
using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.DynamicData;
using Assets.Scripts.Network;
using Assets.Scripts.Saving.Data;
using Assets.Scripts.Saving.Data.InventoryData;
using Assets.Scripts.Saving.Data.LiquidsData;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Assets.Scripts.Core;
using Assets.Scripts.Movement;
using Photon.Pun;
using System.Reflection;
using System;
using Assets.Scripts.Spawning;
using Unity.VisualScripting;
using Assets.Scripts.Saving.EffectsData;

namespace Assets.Scripts.Saving
{
    [RequireComponent(typeof(Summonables))]
    public class GameSaveProvider : MonoBehaviour
    {
        private const string SAVE_PATH = "/Saves";

        public List<Save> Saves { get; private set; } = new List<Save>();


        private string savePath => Path.Combine(SAVE_PATH, Application.persistentDataPath);
        private Summonables summoner => GetComponent<Summonables>();

        private SavedWorld currentWorld;
        private SavedSettings settings;
        private List<SavedElement> worldElements = new List<SavedElement>();

        private void Start()
        {
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            string[] saveFiles = Directory.GetFiles(savePath, "*.json");

            foreach (string filePath in saveFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string fileData = File.ReadAllText(filePath);

                Saves.Add(new Save(fileName, fileData));
            }
        }

        //public void SaveWorld(string saveName)
        //{
        //    if (string.IsNullOrEmpty(saveName))
        //        saveName = $"Save{Saves.Count + 1}";
        //
        //    Save result = null;
        //
        //    SaveableObject[] objects = FindObjectsOfType<SaveableObject>();
        //    Entity[] entities = FindObjectsOfType<Entity>();
        //    DroppedItem[] droppedItems = FindObjectsOfType<DroppedItem>();
        //
        //    List<Entity> listedEntities = new List<Entity>();
        //    List<SaveableObject> listedObjects = new List<SaveableObject>();
        //    List<DroppedItem> listedDroppedItems = new List<DroppedItem>();
        //
        //    if (objects != null)
        //    {
        //        foreach (var obj in objects)
        //            if (obj.gameObject.layer == 7)
        //                listedObjects.Add(obj);
        //    }
        //    if (entities != null)
        //        listedEntities = entities.ToList();
        //
        //    if (droppedItems != null)
        //        listedDroppedItems = droppedItems.ToList();
        //
        //    SavedWorld world = new SavedWorld();
        //
        //    foreach (var obj in listedObjects)
        //    {
        //        if (obj.GetComponent<Entity>() || obj.GetComponent<DroppedItem>())
        //            continue;
        //
        //        Rigidbody rb = obj.GetComponent<Rigidbody>();
        //
        //        if (rb == null)
        //            continue;
        //
        //        SavedVector3 pos = new SavedVector3(
        //            obj.transform.position.x,
        //            obj.transform.position.y,
        //            obj.transform.position.z, worldElements.Count);
        //
        //        SavedVector3 rot = new SavedVector3(
        //            obj.transform.rotation.x,
        //            obj.transform.rotation.y,
        //            obj.transform.rotation.z, worldElements.Count);
        //
        //        SavedVector3 scale = new SavedVector3(
        //            obj.transform.localScale.x,
        //            obj.transform.localScale.y,
        //            obj.transform.localScale.z, worldElements.Count);
        //
        //        SavedTransform sTransform = new SavedTransform(
        //            pos,
        //            rot,
        //            scale
        //            , worldElements.Count);
        //
        //        SavedVector3 velocity = new SavedVector3(
        //            rb.velocity.x,
        //            rb.velocity.y,
        //            rb.velocity.z
        //            , worldElements.Count);
        //
        //        SavedRigidBody rigidbody = new SavedRigidBody(
        //            velocity,
        //            rb.isKinematic,
        //            rb.useGravity
        //            , worldElements.Count);
        //
        //        SavedObject savedObject = new SavedObject(sTransform, rigidbody, obj.ownerUUID, worldElements.Count);
        //        world.objects.Add(savedObject);
        //    }
        //
        //    foreach (var entity in listedEntities)
        //    {
        //        Transform eTransform = entity.transform;
        //        MovementController eMovement = entity.GetComponent<MovementController>();
        //        EntityNetworkData entityNetworkData = entity.GetComponent<EntityNetworkData>();
        //        LiquidContainer liquids = entity.GetComponent<LiquidContainer>();
        //        InventoryDataController inventory = entity.GetComponent<InventoryDataController>();
        //        PhysicsPropertiesContainer physicsProperties = entity.GetComponent<PhysicsPropertiesContainer>();
        //        EffectContainer effects = entity.GetComponent<EffectContainer>();
        //        Rigidbody rb = entity.GetComponent<Rigidbody>();
        //        Summonable summonable = entity.GetComponent<Summonable>();
        //
        //        if (summonable == null || eMovement == null || rb == null || entityNetworkData == null || liquids == null || inventory == null || physicsProperties == null || effects == null)
        //            continue;
        //
        //        List<SavedLqiuid> sLiquids = new List<SavedLqiuid>();
        //
        //        foreach (var liquid in liquids.liquids)
        //            sLiquids.Add(new SavedLqiuid(liquid.type, liquid.amount));
        //
        //        SavedLiquidContainer sLiquidCongainer = new SavedLiquidContainer(sLiquids);
        //
        //        List<SavedItem> sItems = new List<SavedItem>();
        //
        //        foreach (var item in inventory.GetItems)
        //        {
        //            Dictionary<string, float> floatAttributes = new Dictionary<string, float>();
        //            Dictionary<string, int> intAttributes = new Dictionary<string, int>();
        //
        //            if (item is DynamicWeaponData weaponData)
        //            {
        //                intAttributes.Add("ammo", weaponData.currentAmmo);
        //                intAttributes.Add("overallAmmo", weaponData.overallAmmo);
        //                floatAttributes.Add("overheat", weaponData.overheat);
        //            }
        //
        //            sItems.Add(new SavedItem(item.data.Id, floatAttributes, intAttributes, worldElements.Count));
        //        }
        //
        //        SavedInventory sInventory = new SavedInventory(sItems, inventory.CurrentItemIndex, worldElements.Count);
        //
        //        SavedPhysicsProperties sPhysicsProperties = new SavedPhysicsProperties(physicsProperties.Temperature, worldElements.Count);
        //
        //        List<EffectsData.SavedEffect> sEffects = new List<EffectsData.SavedEffect>();
        //
        //        foreach (var effect in effects.Effects)
        //        {
        //            EffectsData.SavedEffect sEffect = new EffectsData.SavedEffect(
        //                effect.RemainingLifetime, 
        //                effect.Amplifier, 
        //                effect.Infinite,
        //                effect.GetType().Name,
        //                worldElements.Count
        //                );
        //
        //            sEffects.Add(sEffect);
        //        }
        //
        //        SavedVector3 sVelocity = new SavedVector3(rb.velocity.x, rb.velocity.y, rb.velocity.z, worldElements.Count);
        //        
        //        SavedRigidBody sRigidbody = new SavedRigidBody(sVelocity, rb.isKinematic, rb.useGravity, worldElements.Count);
        //
        //        SavedVector3 sPosition = new SavedVector3(eTransform.position.x, eTransform.position.y, eTransform.position.z, worldElements.Count);
        //        SavedVector3 sRotation = new SavedVector3(eTransform.rotation.x, eTransform.rotation.y, eTransform.rotation.z, worldElements.Count);
        //        SavedVector3 sScale = new SavedVector3(eTransform.localScale.x, eTransform.localScale.y, eTransform.localScale.z, worldElements.Count);
        //
        //        SavedTransform sTransform = new SavedTransform(sPosition, sRotation, sScale, worldElements.Count);
        //
        //        SavedEntity sEntity = new SavedEntity(
        //            entity.Health,
        //            entity.Invulnerable,
        //            eMovement.IsFlying,
        //            worldElements.Count,
        //            summonable.ObjectId, 
        //            entityNetworkData.UUID,
        //            entityNetworkData.EntityRights,
        //            sLiquidCongainer,
        //            sInventory,
        //            sPhysicsProperties,
        //            entity.DamageSensors,
        //            sEffects,
        //            sRigidbody,
        //            sTransform
        //            );
        //
        //        world.entities.Add(sEntity);
        //    }
        //
        //    foreach (var item in listedDroppedItems)
        //    {
        //        if (item.TryGetComponent<SaveableObject>(out var svObj) && item.TryGetComponent<Rigidbody>(out var rb))
        //        {
        //            Transform iTransform = item.transform;
        //
        //            SavedVector3 sPosition = new SavedVector3(iTransform.position.x, iTransform.position.y, iTransform.position.z, worldElements.Count);
        //            SavedVector3 sRotation = new SavedVector3(iTransform.rotation.x, iTransform.rotation.y, iTransform.rotation.z, worldElements.Count);
        //            SavedVector3 sScale = new SavedVector3(iTransform.localScale.x, iTransform.localScale.y, iTransform.localScale.z, worldElements.Count);
        //
        //            SavedTransform sTransform = new SavedTransform(sPosition, sRotation, sScale, worldElements.Count);
        //
        //            SavedVector3 sVelocity = new SavedVector3(rb.velocity.x, rb.velocity.y, rb.velocity.z, worldElements.Count);
        //
        //            SavedRigidBody sRigidbody = new SavedRigidBody(sVelocity, rb.isKinematic, rb.useGravity, worldElements.Count);
        //
        //            SavedDroppedItem sItem = new SavedDroppedItem(item.data, svObj.ownerUUID, worldElements.Count, sTransform, sRigidbody);
        //
        //            world.droppedItems.Add(sItem);
        //        }
        //    }
        //
        //    string serializedData = JsonConvert.SerializeObject(world, Formatting.Indented);
        //
        //    result = new Save(saveName, serializedData);
        //    Debug.Log(serializedData);
        //    File.WriteAllText(Path.Combine(savePath, saveName + ".json"), serializedData);
        //}
        //public void LoadWorld(int saveIndex)
        //{
        //    if (saveIndex < 0 || saveIndex > Saves.Count - 1)
        //        return;
        //
        //    SavedWorld world = JsonConvert.DeserializeObject<SavedWorld>(Saves[saveIndex].Data);
        //
        //    foreach (var entity in world.entities)
        //    {
        //        if (entity.ownerUUID == settings.playerUUID)
        //        {
        //            Vector3 pos = new Vector3(entity.transform.position.X, entity.transform.position.Y, entity.transform.position.Z);
        //            Vector3 scale = new Vector3(entity.transform.scale.X, entity.transform.scale.Y, entity.transform.scale.Z);
        //            Quaternion rot = new Quaternion(entity.transform.rotation.X, entity.transform.rotation.Y, entity.transform.rotation.Z, 0f);
        //
        //            GameObject entityObj = summoner.Summon(entity.id, pos, scale, rot);
        //
        //            Entity eComponent = entityObj.GetComponent<Entity>();
        //            eComponent.SetHealth(entity.health);
        //            eComponent.Invulnerable = entity.invulnerable;
        //            eComponent.DamageSensors = entity.damageSensors;
        //
        //            LiquidContainer eLiquids = entityObj.GetComponent<LiquidContainer>();
        //            
        //            eLiquids.liquids.Clear();
        //
        //            foreach (var liquid in entity.liquids.Lqiuids)
        //                eLiquids.liquids.Add(new Liquid(liquid.type, liquid.amount));
        //
        //            PhotonView eView = entityObj.GetComponent<PhotonView>();
        //
        //            foreach (var effect in entity.effects)
        //                eView.RPC("RPC_ApplyEffect", RpcTarget.All, JsonUtility.ToJson(effect));
        //        }
        //    }
        //}

        [PunRPC]
        public void WriteElement(int networkId, string jsonData)
        {
            SavedElement findedElement = worldElements.Find(n => n.networkId == networkId);

            if (findedElement != null)
                findedElement = JsonConvert.DeserializeObject<SavedElement>(jsonData);
        }

        [PunRPC]
        public void AddEntity(string jsonData)
        {
            SavedEntity entity = JsonConvert.DeserializeObject<SavedEntity>(jsonData);

            if (worldElements.Find(n => n.networkId == entity.networkId) == null)
            {
                currentWorld.entities.Add(entity);
                worldElements.Add(entity);
            }
        }

        [PunRPC]
        public void AddObject(string jsonData)
        {
            SavedObject obj = JsonConvert.DeserializeObject<SavedObject>(jsonData);

            if (worldElements.Find(n => n.networkId == obj.networkId) == null)
            {
                currentWorld.objects.Add(obj);
                worldElements.Add(obj);
            }
        }

        [PunRPC]
        public void AddDroppedItem(string jsonData)
        {
            SavedDroppedItem item = JsonConvert.DeserializeObject<SavedDroppedItem>(jsonData);

            if (worldElements.Find(n => n.networkId == item.networkId) == null)
            {
                currentWorld.objects.Add(item);
                worldElements.Add(item);
            }
        }
    }
}