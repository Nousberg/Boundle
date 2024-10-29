using Assets.Scripts.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Player
{
    [RequireComponent(typeof(Entity))]
    public class PlayerUiManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image healthImage;

        [Header("Properties")]
        [SerializeField] private float transperencySinAmplitude;
        [SerializeField] private float transperencySinFrequerency;
        [SerializeField] private float transperencyLerpSpeed;

        private Entity player => GetComponent<Entity>();
        private float targetTransperency;
        private float targetSinFrequerency;

        private void Start()
        {
            player.OnDamage += UpdateUi;
        }
        private void Update()
        {
            healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, Mathf.Lerp(healthImage.color.a, targetTransperency + targetTransperency * 0.35f * Mathf.Cos(Time.time * targetSinFrequerency) * transperencySinAmplitude, transperencyLerpSpeed * Time.deltaTime));
        }
        public void UpdateUi(float damage, Entity initiator, DamageType type)
        {
            float healthAspect = 1f - player.Health / player.BaseHealth;
            targetSinFrequerency = transperencySinFrequerency * Mathf.Min(healthAspect, 0.25f);
            targetTransperency = healthAspect;
            healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, targetTransperency + targetTransperency * 0.25f);
        }
    }
}