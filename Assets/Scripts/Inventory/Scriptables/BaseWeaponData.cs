using Assets.Scripts.Entities;
using UnityEngine;

namespace Assets.Scripts.Inventory.Scriptables
{
    [CreateAssetMenu(fileName = "BaseWeaponData", menuName = "ScriptableObjects/Inventory/BaseWeaponData")]
    public class BaseWeaponData : BaseItemData
    {
        [field: Header("Weapon Properties")]
        [field: SerializeField] public WeaponType MyType { get; private set; }
        [field: SerializeField] public DamageType DamageType { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float PushStrenght { get; private set; }
        [field: SerializeField] public float Spread { get; private set; }
        [field: SerializeField] public float Recoil { get; private set; }
        [field: SerializeField] public float FireRate { get; private set; }
        [field: SerializeField] public float ReloadDuration { get; private set; }
        [field: SerializeField] public float Range { get; private set; }
        [field: SerializeField] public int BulletsPerShot { get; private set; }
        [field: SerializeField] public int BaseAmmo { get; private set; }

        [field: Header("Sound Properties")]
        [field: SerializeField] public AudioClip FireSound { get; private set; }
        [field: SerializeField] public AudioClip ReloadSound { get; private set; }

        public enum WeaponType : byte
        {
            Melee,
            Rifle
        }
    }
}