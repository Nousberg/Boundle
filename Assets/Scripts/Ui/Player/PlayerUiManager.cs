using Assets.Scripts.Entities;
using Assets.Scripts.Movement;
using Mirror.Examples.Benchmark;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Player
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(PlayerMovementLogic))]
    public class PlayerUiManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject settingsCanvas;
        [SerializeField] private GameObject playerDataCanvas;
        [SerializeField] private Image healthImage;

        [Header("Properties")]
        [SerializeField] private float transperencySinAmplitude;
        [SerializeField] private float transperencySinFrequerency;
        [SerializeField] private float firstStageTransperencyLerp;
        [SerializeField] private float transperencyLerpSpeed;

        private PlayerMovementLogic playerMovement => GetComponent<PlayerMovementLogic>();
        private Entity player => GetComponent<Entity>();
        private float targetTransperency;
        private float targetSinFrequerency;
        private bool canLerpInUpdate;

        private void Start()
        {
            player.OnHealthChanged += UpdateUi;
        }
        private void Update()
        {
            if (canLerpInUpdate)
                healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, Mathf.Lerp(healthImage.color.a, targetTransperency + targetTransperency * 0.35f * Mathf.Cos(Time.time * targetSinFrequerency) * transperencySinAmplitude, transperencyLerpSpeed * Time.deltaTime));

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                playerDataCanvas.SetActive(!playerDataCanvas.activeInHierarchy);
                settingsCanvas.SetActive(!settingsCanvas.activeInHierarchy);
                playerMovement.lockedDown = !playerMovement.lockedDown;
                Cursor.lockState = playerDataCanvas.activeInHierarchy ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !playerDataCanvas.activeInHierarchy;
            }
        }
        public void UpdateUi()
        {
            float healthAspect = 1f - player.Health / player.BaseHealth;
            targetSinFrequerency = transperencySinFrequerency * Mathf.Min(healthAspect, 0.25f);
            targetTransperency = healthAspect;
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