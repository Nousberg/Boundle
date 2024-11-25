using Assets.Scripts.Entities.Abilities.Scriptables;
using Assets.Scripts.Ui.Player;
using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Entities.Abilities
{
    public abstract class Ability : MonoBehaviour
    {
        [SerializeField] private AbilityData data;

        private bool toggled;

        private void Update()
        {
            if (Input.GetKeyDown(data.KeyBind) && !GameVisualManager.BlockedKeyboard)
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

        public void Activate()
        {
            if (!toggled)
            {
                toggled = true;
                StartCoroutine(nameof(AbilityActivator));
            }
            else
            {
                Deactivate();
            }
        }
        public void Deactivate()
        {
            if (toggled)
            {
                toggled = false;
                StopCoroutine(nameof(AbilityActivator));
                OnDeactivate();
            }
            else
            {
                Activate();
            }
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