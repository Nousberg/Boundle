using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.Ui.Player
{
    public class TouchscreenJoystick : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform joystickParent;
        [SerializeField] private Image joystick;

        [Header("Properties")]
        [SerializeField] private float joystickColorLerpSpeed;
        [SerializeField] private float joystickLerpSpeed;
        [SerializeField] private float joystickRadius;

        public bool CriticalDistance { get; private set; }
        public Vector3 joystickInput { get; private set; }

        private Color targetParentJoystickColor;
        private Color startParentJoystickColor;
        private Color targetJoystickColor;
        private Color startJoystickColor;
        private Vector2 startJoystickPosition;
        private Image joystickParentImage;
        private RectTransform joystickRect => joystick.GetComponent<RectTransform>();

        private void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                joystickParentImage = joystickParent.GetComponent<Image>();
                startJoystickPosition = joystickRect.localPosition;
                startJoystickColor = joystick.color;
                startParentJoystickColor = joystickParentImage.color;
            }
        }

        private void Update()
        {
            if (Application.platform == RuntimePlatform.Android)
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickParent, touch.position, null, out localPoint);

                    Vector2 direction = localPoint - startJoystickPosition;

                    if (direction.magnitude > joystickRadius)
                    {
                        direction = direction.normalized * joystickRadius;
                    }

                    joystickInput = new Vector3(direction.x / joystickRadius, 0f, direction.y / joystickRadius);

                    float touchDistance = Vector2.Distance(localPoint, startJoystickPosition);
                    CriticalDistance = touchDistance > joystickRadius * 0.8f;

                    if (touchDistance < joystickRadius && localPoint.x < Screen.width / 2)
                    {
                        joystickRect.localPosition = Vector3.Lerp(joystickRect.localPosition, startJoystickPosition + direction, joystickLerpSpeed * Time.deltaTime);

                        targetJoystickColor = startJoystickColor;
                        targetJoystickColor.a = targetJoystickColor.a / (touchDistance * 0.05f);
                        joystick.color = Color.Lerp(joystick.color, targetJoystickColor, joystickColorLerpSpeed * Time.deltaTime);

                        targetParentJoystickColor = startParentJoystickColor;
                        targetParentJoystickColor.a = startParentJoystickColor.a / (touchDistance * 0.05f);
                        joystickParentImage.color = Color.Lerp(joystickParentImage.color, targetParentJoystickColor, joystickColorLerpSpeed * Time.deltaTime);
                    }
                    else
                    {
                        ResetJoystick();
                    }
                }
                else
                {
                    ResetJoystick();
                }
        }
        private void ResetJoystick()
        {
            joystickRect.localPosition = Vector3.Lerp(joystickRect.localPosition, startJoystickPosition, joystickLerpSpeed * Time.deltaTime);
            joystickInput = Vector3.zero;

            joystick.color = Color.Lerp(joystick.color, startJoystickColor, joystickColorLerpSpeed * Time.deltaTime);
            joystickParentImage.color = Color.Lerp(joystickParentImage.color, startParentJoystickColor, joystickColorLerpSpeed * Time.deltaTime);
        }
    }
}