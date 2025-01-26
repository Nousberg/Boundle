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

        public void Init()
        {
            needle.OnInjected += HandleInject;
            Physics.IgnoreCollision(needle.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }

        private void HandleInject(Collider collision)
        {
            LiquidContainer container = collision.GetComponent<LiquidContainer>();

            if (container != null)
                liquids.Transfer(container, liquids.Weight * liquidTransferPercenrage * Time.deltaTime);
        }
    }
}
