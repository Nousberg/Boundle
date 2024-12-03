using System;
using UnityEngine;

namespace Assets.Scripts.Entities.Liquids
{
    [RequireComponent(typeof(LiquidContainer))]
    public class Syringe : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Needle needle;

        [Header("Properties")]
        [Range(0f, 1f)][SerializeField] private float liquidTransferPercenrage;

        private LiquidContainer liquids => GetComponent<LiquidContainer>();

        private void Start()
        {
            needle.OnInjected += HandleInject;
            Physics.IgnoreCollision(needle.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }

        private void HandleInject(Collision collision)
        {
            LiquidContainer container = collision.gameObject.GetComponent<LiquidContainer>();

            if (container != null)
            {
                liquids.Transfer(container, liquids.Weight * liquidTransferPercenrage * Time.deltaTime);
            }
        }
    }
}
