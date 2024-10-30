using Assets.Scripts.Entities;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class DamageSourcePrototype : MonoBehaviour
    {
        [SerializeField] private float damageFreqerency;
        [SerializeField] private float damageAmount;
        [SerializeField] private DamageType damageType;

        private float currentDamageTime;

        private void OnCollisionStay(Collision collision)
        {
            if (currentDamageTime <= Time.time)
            {
                currentDamageTime = Time.time + 1f / damageFreqerency;

                Entity e = collision.gameObject.GetComponent<Entity>();
                if (e != null)
                    e.TakeDamage(damageAmount, e, damageType);
            }
        }
    }
}