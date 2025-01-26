using System;
using UnityEngine;

namespace Assets.Scripts.Entities.Liquids
{
    public class Needle : MonoBehaviour
    {
        public event Action<Collider> OnInjected;

        private void OnTriggerStay(Collider other) => OnInjected?.Invoke(other);
    }
}
