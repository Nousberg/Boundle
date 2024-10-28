using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Inventory.Scriptables
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Inventory/Weapon")]
    public class WeaponData : ItemData
    {
        [field: Header("Weapon Properties")]
        [field: SerializeField] public DamageType TypeOfDamage { get; private set; }
        [field: SerializeField] public TypeOfWeapon WeaponType { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float FireRate { get; private set; }
        [field: SerializeField] public float ReloadDuration { get; private set; }
        [field: SerializeField] public float Range { get; private set; }
        [field: SerializeField] public float Recoil { get; private set; }
        [field: SerializeField] public float Spread { get; private set; }
        [field: SerializeField] public float BaseDurability { get; private set; }
        [field: SerializeField] public int BaseAmmo { get; private set; }
        [field: SerializeField] public int BulletsPerShoot { get; private set; }
        [field: SerializeField] public float OverheatRate { get; private set; }
        [field: SerializeField] public float CriticalOverheat { get; private set; }
    }
    public enum TypeOfWeapon : byte
    {
        Bullet,
        Melee
    }
}
