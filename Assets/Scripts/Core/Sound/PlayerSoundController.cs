using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.Core.Sound
{
    public class PlayerSoundController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioSource soundSource;
        [SerializeField] private PlayerMovementLogic movement;
        [SerializeField] private InventoryDataController inventory;

        [Header("Fall Sound Properties")]
        [Min(0f)][SerializeField] private float startTimecode;

        private void Start()
        {
            inventory.OnItemAdded += HandleItemChange;
            inventory.OnItemRemoved += HandleItemChange;
            inventory.OnItemSwitched += HandleItemChange;
        }
        private void HandleItemChange()
        {
            if (inventory.GetItems[inventory.CurrentItemIndex].data is BaseWeaponData baseWeapon)
            {
                WeaponDataController weapon = inventory.AllInGameItems.Find(n => n.BaseData.Id == baseWeapon.Id) as WeaponDataController;
                if (weapon != null)
                {
                    weapon.OnFire -= HandleFire;
                    weapon.OnFire += HandleFire;

                    weapon.OnReload -= HandleReload;
                    weapon.OnReload += HandleReload;
                }
            }
        }
        private void HandleFire()
        {
            if (inventory.GetItems[inventory.CurrentItemIndex].data is BaseWeaponData weaponData)
                SoundManager.Play(soundSource, weaponData.FireSound, 1f, 1f);
        }
        private void HandleReload()
        {
            if (inventory.GetItems[inventory.CurrentItemIndex].data is BaseWeaponData weaponData)
                SoundManager.Play(soundSource, weaponData.ReloadSound, 1f, 1f);
        }
    }
}
