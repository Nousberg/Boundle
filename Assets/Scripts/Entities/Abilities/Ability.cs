using Assets.Scripts.Entities.Abilities.Scriptables;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Entities.Abilities
{
    public abstract class Ability : MonoBehaviour
    {
        [SerializeField] private AbilityData data;

        private bool toggled;

        private void Update()
        {
            if (Input.GetKeyDown(data.KeyBind))
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