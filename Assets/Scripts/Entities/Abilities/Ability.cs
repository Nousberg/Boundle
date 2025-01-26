using Assets.Scripts.Entities.Abilities.Scriptables;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Entities.Abilities
{
    public abstract class Ability : MonoBehaviour
    {
        [SerializeField] private AbilityData data;

        private PhotonView view => GetComponent<PhotonView>();

        private bool toggled;

        private void Update()
        {
            if (view.IsMine)
            {
                if (Keyboard.current[data.KeyBind].wasPressedThisFrame)
                {
                    toggled = !toggled;

                    if (toggled)
                        StartCoroutine(nameof(AbilityActivator));
                    if (!toggled)
                        OnDeactivate();
                }

                if (toggled)
                    SafeUpdate();
            }
        }

        public void Activate()
        {
            toggled = true;
            StartCoroutine(nameof(AbilityActivator));
        }
        public void Deactivate()
        {
            toggled = false;
            StopCoroutine(nameof(AbilityActivator));
            OnDeactivate();
        }

        private IEnumerator AbilityActivator()
        {
            yield return new WaitForSeconds(data.StartDelay);
            ToggleAbility();
            yield return new WaitForSeconds(data.Duration);
            OnDeactivate();
        }

        protected abstract void OnDeactivate();
        protected abstract void ToggleAbility();
        protected virtual void SafeUpdate() { }
    }
}