using Assets.Scripts.Entities;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(PhotonView))]
    public class DamageSourcePrototype : MonoBehaviour
    {
        [SerializeField] public float damageFrequency;
        [SerializeField] public float damageAmount;
        [SerializeField] public DamageData.DamageType damageType;

        private Entity thisEntity => GetComponent<Entity>();
        private PhotonView view => GetComponent<PhotonView>();

        private float currentDamageTime;

        private void OnCollisionStay(Collision collision)
        {
            if (view.IsMine && currentDamageTime <= Time.time)
            {
                currentDamageTime = Time.time + 1f / damageFrequency;

                Entity e = collision.gameObject.GetComponent<Entity>();
                PhotonView view = collision.gameObject.GetComponent<PhotonView>();

                if (e != null && view != null)
                    e.TakeDamage(damageAmount, thisEntity, damageType);
            }
        }
    }
}