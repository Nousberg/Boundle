using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Entities.Abilities.Scriptables;
using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Entities.Abilities
{
    public abstract class Ability : MonoBehaviour
    {
        [SerializeField] private AbilityData data;

        private PhotonView view => GetComponent<PhotonView>();

        private bool toggled;
        private InputState inputSource;

        private void IfToggled(InputHandler.InputBind bind)
        {
            if (bind == data.KeyBind)
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

        public void Init(InputState inputSource)
        {
            this.inputSource = inputSource;
            inputSource.InputRecieved += IfToggled;
            OnInit();
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

            if (data.Duration != float.PositiveInfinity)
            {
                yield return new WaitForSeconds(data.Duration);
                OnDeactivate();
            }
        }

        protected abstract void OnDeactivate();
        protected abstract void ToggleAbility();
        protected virtual void OnInit() { }
        protected virtual void SafeUpdate() { }

        private void OnDestroy()
        {
            inputSource.InputRecieved -= IfToggled;
        }
    }
}
