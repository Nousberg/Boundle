using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Entities;
using Assets.Scripts.Movement;
using DG.Tweening;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Player
{
    public class GameVisualManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridLayoutGroup aim;
        [SerializeField] private Image healthImage;
        [SerializeField] private GameObject chatBox;
        [SerializeField] private GameObject mobileUiElements;
        [SerializeField] private CanvasGroup mainUiGroup;
        [SerializeField] private CanvasGroup settingsUiGroup;
        [SerializeField] private CanvasGroup aimUiGroup;

        [Header("Properties")]
        [SerializeField] private float defaultAimTransperency;
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

        private RectTransform settingsRect => settingsUiGroup.GetComponent<RectTransform>();
        private PlayerMovementLogic playerMovement => GetComponent<PlayerMovementLogic>();
        private Entity player => GetComponent<Entity>();
        private PhotonView view => GetComponent<PhotonView>();

        private InputMachine inputMachine;
        private Sequence settingsAnim;
        private float defaultMainUiAlpha;
        private float defaultSettingsUiAplha;
        private float targetTransperency;
        private float targetSinFrequerency;
        private bool canUpdateBloodScreen = true;
        private bool canLerpInUpdate;
        private Vector2 startSpacing;
        private Vector2 startCellSize;
        private bool settingsEnabled;
        private bool isChatOpen;

        public void Init(InputMachine inputMachine)
        {
            this.inputMachine = inputMachine;

            defaultMainUiAlpha = mainUiGroup.alpha;
            defaultSettingsUiAplha = settingsUiGroup.alpha;

            player.OnHealthChanged += UpdateHealthUI;

            startSpacing = aim.spacing;
            startCellSize = aim.cellSize;

            if (Application.platform != RuntimePlatform.Android)
                mobileUiElements.SetActive(false);
        }
        private void Update()
        {
            if (view.IsMine)
            {
                float velocity = Mathf.Clamp(playerMovement.CurrentVelocity * 0.25f, 1f, 6f);

                aimUiGroup.alpha = Mathf.Lerp(aimUiGroup.alpha, defaultAimTransperency / velocity, aimLerpSpeed * Time.deltaTime);

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
                    inputMachine.blockInput = false;
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
        }

        private void ToggleCursor(bool state)
        {
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = state;
        }
        private void ToggleChat(bool value)
        {
            if (value)
                inputMachine.blockInput = true;

            isChatOpen = value;
            chatBox.SetActive(value);
            aim.gameObject.SetActive(!value);
        }
        private void ToggleSettings(bool value)
        {
            if (value)
            {
                inputMachine.blockInput = true;
                ToggleChat(false);
            }

            settingsEnabled = value;

            ToggleCursor(value);

            StartSettingsAnimation();
        }
        private void StartSettingsAnimation()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            settingsAnim.Kill();

            if (!settingsEnabled)
            {
                mainUiGroup.gameObject.SetActive(true);

                settingsAnim = DOTween.Sequence();

                settingsAnim.Append(
                    mainUiGroup.DOFade(defaultMainUiAlpha, closeDuration));

                settingsAnim.Join(
                    settingsUiGroup.DOFade(0f, closeDuration));

                settingsAnim.Join(
                    settingsRect.DOScale(Vector2.one * closedStateScaleOffset, closeDuration));

                settingsAnim.Join(
                    settingsRect.DOMove(screenCenter * closedStatePositionOffset, closeDuration));

                settingsAnim.OnComplete(() => { 
                    settingsUiGroup.gameObject.SetActive(false);
                } )
                        .SetUpdate(UpdateType.Normal, true)
                        .SetEase(closeEase);
            }
            else
            {
                settingsUiGroup.gameObject.SetActive(true);

                settingsAnim = DOTween.Sequence();

                settingsAnim.Append(
                    settingsRect.DOScale(Vector2.one, openDuration));

                settingsAnim.Join(
                    mainUiGroup.DOFade(0f, openDuration));

                settingsAnim.Join(
                    settingsUiGroup.DOFade(defaultSettingsUiAplha, openDuration));

                settingsAnim.Join(
                    settingsRect.DOMove(screenCenter, openDuration));

                settingsAnim.OnComplete(() => {
                    mainUiGroup.gameObject.SetActive(false);
                })
                        .SetEase(openEase)
                        .SetUpdate(UpdateType.Normal, true);
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
