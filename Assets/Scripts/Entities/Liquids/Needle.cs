using System;
using UnityEngine;

namespace Assets.Scripts.Entities.Liquids
{
    public class Needle : MonoBehaviour
    {
        public event Action<Collision> OnInjected;

        private void OnCollisionStay(Collision collision) => OnInjected?.Invoke(collision);
    }
}
