using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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