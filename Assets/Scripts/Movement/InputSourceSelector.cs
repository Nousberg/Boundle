using Assets.Scripts.Core.InputSystem;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class InputSourceSelector : MonoBehaviour
    {
        public bool touchInput;

        [SerializeField] private InputHandler input;

        private void Start()
        {
        }
    }
}