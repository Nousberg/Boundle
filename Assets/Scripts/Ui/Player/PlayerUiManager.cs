using Assets.Scripts.Entities;
using Assets.Scripts.Movement;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Player
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(PlayerMovementLogic))]
    public class PlayerUiManager : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup aim;
        [SerializeField] private GameObject settingsBackgroundContainer;
        [SerializeField] private GameObject playerDataCanvas;
        [SerializeField] private Image healthImage;
        [SerializeField] private GameObject[] MobileUiElements;
        [SerializeField] private Image[] aimParts;

        [Header("Properties")]
        [SerializeField] private Color defaultAimColor;
        [SerializeField] private float transperencySinAmplitude;
        [SerializeField] private float transperencySinFrequerency;
        [SerializeField] private float firstStageTransperencyLerp;
        [SerializeField] private float transperencyLerpSpeed;
        [SerializeField] private float aimLerpSpeed;

        [Header("Settings Animation")]
        [SerializeField] private Ease openEase;
        [SerializeField] private Ease closeEase;
        [Min(0f)][SerializeField] private float closeDuration;
        [Min(0f)][SerializeField] private float openDuration;
        [Range(0f, 1f)][SerializeField] private float closedStateScaleOffset;
        [SerializeField] private Vector2 closedStatePositionOffset;

        private PlayerMovementLogic playerMovement => GetComponent<PlayerMovementLogic>();
        private RectTransform settingsRect => settingsBackgroundContainer.GetComponent<RectTransform>();
        private Entity player => GetComponent<Entity>();
        private float targetTransperency;
        private float targetSinFrequerency;
        private bool canLerpInUpdate;
        private Vector2 startSpacing;
        private Vector2 startCellSize;
        private bool settingsEnabled;

        private void Start()
        {
            player.OnHealthChanged += UpdateHealthUI;

            startSpacing = aim.spacing;
            startCellSize = aim.cellSize;

            if (Application.platform != RuntimePlatform.Android)
                foreach (var element in MobileUiElements)
                {
                    element.SetActive(false);
                }
        }
        private void Update()
        {
            float velocity = Mathf.Clamp(playerMovement.CurrentVelocity * 0.05f, 1f, 2f);
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
                startCellSize / velocity,
                aimLerpSpeed * Time.deltaTime);

            if (canLerpInUpdate)
                healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, Mathf.Lerp(healthImage.color.a, targetTransperency + targetTransperency * 0.35f * Mathf.Cos(Time.time * targetSinFrequerency) * transperencySinAmplitude, transperencyLerpSpeed * Time.deltaTime));

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                settingsEnabled = !settingsEnabled;

                playerMovement.lockedDown = settingsEnabled;
                Cursor.lockState = !settingsEnabled ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = settingsEnabled;

                StartSettingsAnimation();
            }
        }
        private void StartSettingsAnimation()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            if (!settingsEnabled)
            {
                settingsRect.DOScale(Vector2.one * closedStateScaleOffset, closeDuration)
                    .SetUpdate(UpdateType.Normal, true)
                    .SetEase(closeEase)
                    .OnComplete(() =>
                    {
                        settingsBackgroundContainer.SetActive(false);
                        playerDataCanvas.SetActive(true);
                    });
                settingsRect.DOMove(screenCenter * closedStatePositionOffset, openDuration)
                    .SetUpdate(UpdateType.Normal, true)
                    .SetEase(closeEase);
            }
            else
            {
                settingsRect.DOScale(Vector2.one, openDuration)
                    .SetUpdate(UpdateType.Normal, true)
                    .SetEase(openEase)
                    .OnStart(() =>
                    {
                        playerDataCanvas.SetActive(false);
                        settingsBackgroundContainer.SetActive(true);
                    });
                settingsRect.DOMove(screenCenter, openDuration)
                    .SetUpdate(UpdateType.Normal, true)
                    .SetEase(openEase);
            }
        }
        private void UpdateHealthUI()
        {
            float healthAspect = 1f - player.Health / player.BaseHealth;
            targetSinFrequerency = transperencySinFrequerency * Mathf.Min(healthAspect, 0.25f);
            targetTransperency = healthAspect;
            StopAllCoroutines();
            StartCoroutine(TransperencyLerpFirstStage(targetTransperency + targetTransperency * 0.25f));
        }
        private IEnumerator TransperencyLerpFirstStage(float value)
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