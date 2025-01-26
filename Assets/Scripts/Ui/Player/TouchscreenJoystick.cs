using Assets.Scripts.Core.InputSystem;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.Ui.Player
{
    public class TouchscreenJoystick : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputHandler input;
        [SerializeField] private Image joystickBorderImg;
        [SerializeField] private Image joystickDirectionImg;

        [Header("Properties")]
        [SerializeField] private float joystickDirectionLerpSpeed;
    }
}