using Assets.Scripts.Effects;
using Assets.Scripts.Effects.Movement;
using Assets.Scripts.Inventory.DynamicData;
using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Movement;
using Photon.Pun;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Saving.Data.InventoryData;
using Assets.Scripts.Network;

namespace Assets.Scripts.Inventory
{
    public class InventoryDataController : MonoBehaviourPun, IPunObservable
    {
        [Header("References")]
        [SerializeField] private EffectContainer effects;
        [SerializeField] private MovementController movement;

        [field: Header("References")]
        [field: SerializeField] public List<ItemDataController> AllInGameItems { get; private set; } = new List<ItemDataController>();
        [field: SerializeField] public List<ItemDataController> DefaultItems { get; private set; } = new List<ItemDataController>();
        [field: Min(0f)][field: SerializeField] public float MaxInventoryWeight { get; private set; }

        public event Action OnItemSwitched;
        public event Action<int> OnClear;
        public event Action<string> OnItemAdded;
        public event Action<string> OnItemRemoved;

        public int CurrentItemIndex { get; private set; }

        public float InventoryWeight => aviableItems.Sum(n => n.data.Weight);
        public List<DynamicItemData> GetItems => new List<DynamicItemData>(aviableItems);

        private PhotonView view => GetComponent<PhotonView>();

        private List<DynamicItemData> aviableItems = new List<DynamicItemData>();
        private Effect cachedSlowdown;

        public void Init()
        {
            bool isPlayer = GetComponent<PlayerNetworkManager>();

            foreach (var item in DefaultItems)
            {
                if (item.BaseData is not BaseWeaponData data)
                {
                    if ((PhotonNetwork.IsMasterClient && isPlayer) || item.BaseData.Id != 1)
                        aviableItems.Add(new DynamicItemData(item.BaseData));

                    continue;
                }

                aviableItems.Add(new DynamicWeaponData(data, data.BaseAmmo, data.BaseAmmo, 0f));
            }

            SwitchItem(0);
            ApplySlowDown();
        }

        public void SwitchItem(int index)
        {
            if (index < 0 || index >= aviableItems.Count)
                return;

            AllInGameItems.Find(n => n.BaseData.Id == aviableItems[index].data.Id).InjectData(aviableItems[index]);

            CurrentItemIndex = index;
            OnItemSwitched?.Invoke();

            ApplySlowDown();
        }
        public bool TryAddItem(DynamicItemData item)
        {
            if (InventoryWeight + item.data.Weight <= MaxInventoryWeight)
            {
                if (item is DynamicWeaponData weaponData)
                {
                    Dictionary<string, int> ints = new Dictionary<string, int>();
                    ints.Add("ammo", weaponData.currentAmmo);
                    ints.Add("overallAmmo", weaponData.overallAmmo);

                    Dictionary<string, float> floats = new Dictionary<string, float>();
                    floats.Add("overheat", weaponData.overheat);

                    SavedItem sWeapon = new SavedItem(item.data.Id, floats, ints, -1);

                    view.RPC(nameof(RPC_AddItem), view.Owner, JsonConvert.SerializeObject(sWeapon));
                    AfterInventoryChange();
                    OnItemAdded?.Invoke(item.data.name);
                    return true;
                }

                SavedItem sItem = new SavedItem(item.data.Id, new Dictionary<string, float>(), new Dictionary<string, int>(), -1);
                view.RPC(nameof(RPC_AddItem), view.Owner, JsonConvert.SerializeObject(sItem));
                AfterInventoryChange();
                OnItemAdded?.Invoke(item.data.name);
                return true;
            }

            return false;
        }
        public bool TryAddItemByIndex(int id)
        {
            ItemDataController reference = AllInGameItems.Find(n => n.BaseData.Id == id);

            if (reference != null && InventoryWeight + reference.BaseData.Weight <= MaxInventoryWeight)
            {
                view.RPC(nameof(RPC_AddItemByIndex), view.Owner, id);
                AfterInventoryChange();
                OnItemAdded?.Invoke(reference.BaseData.name);
                return true;
            }
            return false;
        }
        public bool TryRemoveItem(int index)
        {
            if (index < 0 || index >= aviableItems.Count || !aviableItems[index].data.Dropable)
                return false;

            string itemName = aviableItems[index].data.name;

            view.RPC(nameof(RPC_RemoveItem), view.Owner, index);
            AfterInventoryChange();
            OnItemRemoved?.Invoke(itemName);
            return true;
        }
        public void Clear()
        {
            foreach (var item in GetItems)
                if (item.data.Dropable)
                {
                    OnClear?.Invoke(aviableItems.IndexOf(item));
                    aviableItems.Remove(item);
                }

            AfterInventoryChange();
        }

        [PunRPC]
        private void RPC_RemoveItem(int index)
        {
            aviableItems.RemoveAt(index);
        }
        [PunRPC]
        private void RPC_AddItem(string jsonData)
        {
            SavedItem sItem = JsonConvert.DeserializeObject<SavedItem>(jsonData);
            ItemDataController reference = AllInGameItems.Find(n => n.BaseData.Id == sItem.id);
            
            if (reference is WeaponDataController weaponController)
            {
                float overheat = sItem.floatAttributes["overheat"];
                int ammo = sItem.intAttributes["ammo"];
                int overallAmmo = sItem.intAttributes["overallAmmo"];

                aviableItems.Add(new DynamicWeaponData(reference.BaseData, ammo, overallAmmo, overheat));
                return;
            }

            aviableItems.Add(new DynamicItemData(reference.BaseData));
        }
        [PunRPC]
        private void RPC_AddItemByIndex(int id)
        {
            ItemDataController reference = AllInGameItems.Find(n => n.BaseData.Id == id);

            if (reference != null)
            {
                if (reference.BaseData is not BaseWeaponData weaponData)
                {
                    aviableItems.Add(new DynamicItemData(reference.BaseData));
                    AfterInventoryChange();
                    return;
                }

                aviableItems.Add(new DynamicWeaponData(reference.BaseData, weaponData.BaseAmmo, weaponData.BaseAmmo, 0f));
                AfterInventoryChange();
            }
        }
        private void ApplySlowDown()
        {
            if (movement != null && effects != null)
            {
                if (cachedSlowdown != null && effects.Effects.Contains(cachedSlowdown))
                    cachedSlowdown.SetAmplifier(aviableItems[CurrentItemIndex].data.Weight);
                else
                {
                    cachedSlowdown = new Slowdown(movement, 0, aviableItems[CurrentItemIndex].data.Weight, true);
                    effects.ApplyEffect(cachedSlowdown);
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(CurrentItemIndex);
                stream.SendNext(aviableItems.Count);

                foreach (var item in aviableItems)
                {
                    if (item is not DynamicWeaponData weapon)
                    {
                        stream.SendNext(item.data.Id);
                        continue;
                    }

                    Dictionary<string, int> ints = new Dictionary<string, int>();
                    ints.Add("ammo", weapon.currentAmmo);
                    ints.Add("overallAmmo", weapon.overallAmmo);

                    Dictionary<string, float> floats = new Dictionary<string, float>();
                    floats.Add("overheat", weapon.overallAmmo);

                    SavedItem sItem = new SavedItem(item.data.Id, floats, ints, -1);

                    stream.SendNext(JsonConvert.SerializeObject(sItem));
                }
            }
            else
            {
                CurrentItemIndex = (int)stream.ReceiveNext();
                aviableItems.Clear();

                int items = (int)stream.ReceiveNext();
                for (int i = 0; i < items; i++)
                {
                    object packet = stream.ReceiveNext();

                    if (packet is not string weaponPacket)
                    {
                        ItemDataController reference = AllInGameItems.Find(n => n.BaseData.Id == (int)packet);

                        if (reference != null)
                            aviableItems.Add(new DynamicItemData(reference.BaseData));

                        continue;
                    }

                    SavedItem sItem = JsonConvert.DeserializeObject<SavedItem>(weaponPacket);

                    ItemDataController weaponReference = AllInGameItems.Find(n => n.BaseData.Id == sItem.id);

                    if (weaponReference != null)
                    {
                        DynamicWeaponData weapon = new DynamicWeaponData(weaponReference.BaseData, sItem.intAttributes["ammo"], sItem.intAttributes["overallAmmo"], sItem.floatAttributes["overheat"]);
                        aviableItems.Add(weapon);
                    }
                }
            }
        }

        private void AfterInventoryChange() => SwitchItem(aviableItems.Count - 1);
    }
}