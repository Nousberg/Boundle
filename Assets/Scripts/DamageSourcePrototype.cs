using Assets.Scripts.Entities;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Entity))]
    public class DamageSourcePrototype : MonoBehaviour
    {
        [SerializeField] public float damageFrequency;
        [SerializeField] public float damageAmount;
        [SerializeField] public DamageType damageType;

        private float currentDamageTime;
        private Entity thisEntity => GetComponent<Entity>();

        private void OnCollisionStay(Collision collision)
        {
            if (currentDamageTime <= Time.time)
            {
                currentDamageTime = Time.time + 1f / damageFrequency;

                Entity e = collision.gameObject.GetComponent<Entity>();
                if (e != null)
                    e.TakeDamage(damageAmount, thisEntity, damageType);
            }
        }
    }
}