using Assets.Scripts.Entities;
using Assets.Scripts.Movement;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Player
{
    public class GameVisualManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerObj;

        [Header("References")]
        [SerializeField] private GridLayoutGroup aim;
        [SerializeField] private GameObject settingsUi;
        [SerializeField] private Image healthImage;
        [SerializeField] private Volume postProcessing;
        [SerializeField] private TextMeshProUGUI chatBoxText;
        [SerializeField] private Image chatBoxBackground;
        [SerializeField] private GameObject chatBox;
        [SerializeField] private GameObject mobileUiElements;
        [SerializeField] private TextMeshProUGUI[] mainUiTexts;
        [SerializeField] private Image[] mainUiImages;
        [SerializeField] private Image[] aimParts;

        [Header("Properties")]
        [SerializeField] private Color defaultAimColor;
        [SerializeField] private float transperencySinAmplitude;
        [SerializeField] private float transperencySinFrequerency;
        [SerializeField] private float firstStageTransperencyLerp;
        [SerializeField] private float transperencyLerpSpeed;
        [SerializeField] private float aimLerpSpeed;

        [Header("Chat Box Properties")]
        [SerializeField] private float widthPadding;
        [SerializeField] private float heightPadding;
        [SerializeField] private KeyCode chatOpenBind;

        [Header("Settings Menu Animation")]
        [SerializeField] private Ease openEase;
        [SerializeField] private Ease closeEase;
        [Min(0f)][SerializeField] private float closeDuration;
        [Min(0f)][SerializeField] private float openDuration;
        [Range(0f, 1f)][SerializeField] private float closedStateScaleOffset;
        [SerializeField] private Vector2 closedStatePositionOffset;

        public static bool BlockedKeyboard { get; private set; }

        private RectTransform settingsRect => settingsUi.GetComponent<RectTransform>();
        private PlayerMovementLogic playerMovement;
        private Entity player;

        private float targetTransperency;
        private float targetSinFrequerency;
        private bool canUpdateBloodScreen = true;
        private bool canLerpInUpdate;
        private Vector2 startSpacing;
        private Vector2 startCellSize;
        private bool settingsEnabled;
        private bool isChatOpen;

        private void Start()
        {
            player = playerObj.GetComponent<Entity>();
            playerMovement = playerObj.GetComponent<PlayerMovementLogic>();

            player.OnHealthChanged += UpdateHealthUI;

            startSpacing = aim.spacing;
            startCellSize = aim.cellSize;

            chatBoxText.enableWordWrapping = true;

            if (Application.platform != RuntimePlatform.Android)
                mobileUiElements.SetActive(false);
            else
                postProcessing.enabled = false;
        }
        private void Update()
        {
            float velocity = Mathf.Clamp(playerMovement.CurrentVelocity * 0.25f, 1f, 6f);
            foreach (var part in aimParts)
            {
                Color targetColor = defaultAimColor;
                targetColor.a /= velocity;
                part.color = Color.Lerp(part.color, targetColor, aimLerpSpeed * Time.deltaTime);
            }
            aim.spacing = Vector2.Lerp(
                aim.spacing,
                startSpacing + startSpacing * playerMovement.CurrentVelocity,
                aimLerpSpeed * Time.deltaTime);
            aim.cellSize = Vector2.Lerp(
                aim.cellSize,
                startCellSize * Mathf.Clamp(playerMovement.CurrentVelocity * 0.15f, 1f, 2f),
                aimLerpSpeed * Time.deltaTime);

            if (canLerpInUpdate && canUpdateBloodScreen)
                healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, Mathf.Lerp(healthImage.color.a, targetTransperency + targetTransperency * 0.35f * Mathf.Cos(Time.time * targetSinFrequerency) * transperencySinAmplitude, transperencyLerpSpeed * Time.deltaTime));

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BlockedKeyboard = false;

                ToggleCursor(false);

                if (!isChatOpen)
                    ToggleSettings(!settingsEnabled);

                ToggleChat(false);
            }
            else if (Input.GetKeyDown(chatOpenBind))
            {
                if (!settingsEnabled)
                {
                    ToggleChat(true);
                    ToggleCursor(true);
                }
            }
        }

        public void UpdateChatBoxSize()
        {
            Bounds messageBounds = chatBoxText.textBounds;

            Vector2 targetSize;
            targetSize.x = messageBounds.size.x + widthPadding;
            targetSize.y = messageBounds.size.y + heightPadding;

            chatBoxBackground.rectTransform.sizeDelta = targetSize;
        }

        private void ToggleCursor(bool state)
        {
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = state;
        }
        private void ToggleChat(bool value)
        {
            if (value)
                BlockedKeyboard = true;

            isChatOpen = value;
            chatBox.SetActive(value);
            aim.gameObject.SetActive(!value);
        }
        private void ToggleSettings(bool value)
        {
            if (value)
                ToggleChat(false);

            settingsEnabled = value;

            BlockedKeyboard = value;
            ToggleCursor(value);

            StartSettingsAnimation();
        }
        private void StartSettingsAnimation()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            if (!settingsEnabled)
            {
                foreach (var element in mainUiImages)
                {
                    Color targetColor = element.color;
                    element.color = Color.clear;

                    canUpdateBloodScreen = false;

                    element.gameObject.SetActive(true);

                    element.DOColor(targetColor, closeDuration)
                    .SetUpdate(UpdateType.Normal, true)
                    .SetEase(closeEase)
                    .OnComplete(() =>
                    {
                        canUpdateBloodScreen = true;
                    });
                }
                foreach (var element in mainUiTexts)
                {
                    Color targetColor = element.color;
                    element.color = Color.clear;

                    canUpdateBloodScreen = false;

                    element.gameObject.SetActive(true);

                    element.DOColor(targetColor, closeDuration)
                    .SetUpdate(UpdateType.Normal, true)
                    .SetEase(closeEase)
                    .OnComplete(() =>
                    {
                        canUpdateBloodScreen = true;
                    });
                }
                settingsRect.DOScale(Vector2.one * closedStateScaleOffset, closeDuration)
                    .SetEase(closeEase)
                    .OnComplete(() => settingsUi.SetActive(false));
                settingsRect.DOMove(screenCenter * closedStatePositionOffset, closeDuration)
                    .SetEase(closeEase);
            }
            else
            {
                foreach (var element in mainUiImages)
                {
                    Color startColor = element.color;
                    Color targetColor = element.color;
                    targetColor.a = 0f;

                    canUpdateBloodScreen = false;

                    element.DOColor(targetColor, openDuration)
                    .SetUpdate(UpdateType.Normal, true)
                    .SetEase(openEase)
                    .OnComplete(() =>
                    {
                        element.color = startColor;
                        element.gameObject.SetActive(false);
                    });
                }
                foreach (var element in mainUiTexts)
                {
                    Color startColor = element.color;
                    Color targetColor = element.color;
                    targetColor.a = 0f;

                    canUpdateBloodScreen = false;

                    element.DOColor(targetColor, openDuration)
                    .SetUpdate(UpdateType.Normal, true)
                    .SetEase(openEase)
                    .OnComplete(() =>
                    {
                        element.color = startColor;
                        element.gameObject.SetActive(false);
                    });
                }

                settingsUi.SetActive(true);
                settingsRect.DOScale(Vector2.one, openDuration).SetEase(openEase);
                settingsRect.DOMove(screenCenter, openDuration).SetEase(openEase);
            }
        }

        private void UpdateHealthUI()
        {
            float healthAspect = 1f - player.Health / player.BaseHealth;
            targetSinFrequerency = transperencySinFrequerency * Mathf.Min(healthAspect, 0.25f);
            targetTransperency = healthAspect;

            if (canUpdateBloodScreen)
            {
                StopAllCoroutines();
                StartCoroutine(HealthTransperencyLerpFirstStage(targetTransperency + targetTransperency * 0.25f));
            }
        }

        private IEnumerator HealthTransperencyLerpFirstStage(float value)
        {
            canLerpInUpdate = false;
            while (Mathf.RoundToInt(healthImage.color.a * 10f) < Mathf.RoundToInt(value * 10f))
            {
                healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, Mathf.Lerp(healthImage.color.a, targetTransperency + targetTransperency * 0.25f, firstStageTransperencyLerp * Time.deltaTime));
                yield return null;
            }
            canLerpInUpdate = true;
        }
    }
}
