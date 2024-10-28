using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Entities.Abilities
{
    public abstract class Ability : MonoBehaviour
    {
        [SerializeField] private KeyCode KeyBind;
        [SerializeField] private float Duration;
        [SerializeField] private float Cooldown;
        [SerializeField] private float DelayBeforeStart;

        private bool toggled;

        private void Update()
        {
            if (Input.GetKeyDown(KeyBind))
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
            yield return new WaitForSeconds(DelayBeforeStart);
            ToggleAbility();
            yield return new WaitForSeconds(Duration);
            OnDeactivate();
        }

        protected abstract void OnDeactivate();
        protected abstract void ToggleAbility();
        protected virtual void SafeUpdate() { }
    }
}