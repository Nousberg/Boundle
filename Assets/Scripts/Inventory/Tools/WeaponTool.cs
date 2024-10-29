using Assets.Scripts.Entities;
using Assets.Scripts.Inventory.ItemTypes;
using Assets.Scripts.Inventory.Scriptables;
using UnityEngine;

namespace Assets.Scripts.Inventory.Tools
{
    public static class WeaponTool
    {
        public static void Reload(WeaponItem weaponContainer, WeaponData weaponData)
        {
            int v = weaponContainer.maxAmmo;
            weaponContainer.maxAmmo -= Mathf.Clamp(weaponData.BaseAmmo - weaponContainer.ammo, 0, weaponContainer.maxAmmo);
            weaponContainer.ammo += Mathf.Clamp(weaponData.BaseAmmo - weaponContainer.ammo, 0, v);
        }
        public static void Fire(WeaponData data, Entity initiator, Transform scanPosition)
        {
            RaycastHit hit;

            for (int i = 0; i < data.BulletsPerShoot; i++)
                if (Physics.Raycast(scanPosition.position, scanPosition.forward, out hit, data.Range))
                {
                    Entity entity = hit.collider.GetComponent<Entity>();

                    if (entity != null)
                    {
                        entity.TakeDamage(data.Damage, initiator, data.TypeOfDamage);
                    }
                }
        }
    }
}
