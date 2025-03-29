using Assets.Scripts.Effects;
using Assets.Scripts.Ui.Effects.Sciptables;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts.Ui.Effects
{
    public class PlayerEffectsVisualizer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform effectsParent;
        [SerializeField] private List<EffectUiData> effectsUiData = new List<EffectUiData>();

        [Header("Properties")]
        [SerializeField] private Color backgroundEffectColor;
        [SerializeField] private Ease disaspearedEffectEase;
        [SerializeField] private float disaspearedEffectAnimDuration;
        [SerializeField] private float disaspearedEffectAnimOpacitySpeed;
        [SerializeField] private Vector3 disaspearedEffectOffset;
        [Min(0)][SerializeField] private int maxVisualizedEffects;

        private EffectContainer effectContainer;

        private List<GameObject> backgroundEffectObjects = new List<GameObject>();
        private List<DynamicEffectUiData> effectObjects = new List<DynamicEffectUiData>();

        public void Init(EffectContainer effectContainer)
        {
            this.effectContainer = effectContainer;

            effectContainer.OnEffectAdded += (e) => { VisualizeEffects(); };
            effectContainer.OnEffectRemoved += EffectDisaspearAnim;
        }

        private void Update()
        {
            foreach (var effect in effectObjects)
            {
                if (!effectContainer.Effects.Contains(effect.effect))
                    continue;

                effect.icon.fillAmount = Mathf.Lerp(effect.icon.fillAmount, (float)effect.effect.RemainingLifetime / effect.effect.Duration, disaspearedEffectAnimOpacitySpeed * Time.deltaTime);
            }
        }
        private IEnumerator HandleEffectEndOpacity(Image effect)
        {
            while (effect != null && effect.fillAmount > 0.01f)
            {
                effect.fillAmount = Mathf.Lerp(effect.fillAmount, 0f, disaspearedEffectAnimOpacitySpeed * Time.deltaTime);
                yield return null;
            }
        }
        private void EffectDisaspearAnim(Effect effect)
        {
            DynamicEffectUiData effectUi = effectObjects.Find(x => x.effect == effect);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(effectUi.icon.transform
                .DOMove(effectUi.icon.transform.position + disaspearedEffectOffset, disaspearedEffectAnimDuration)
                );

            sequence.Join(effectUi.icon.GetComponent<CanvasGroup>().DOFade(0f, disaspearedEffectAnimDuration));

            sequence.SetUpdate(UpdateType.Normal, true)
                .SetEase(disaspearedEffectEase)
                .OnComplete(() =>
                {
                    Destroy(effectUi.icon.gameObject);
                    effectObjects.Remove(effectUi);
                });

            StartCoroutine(HandleEffectEndOpacity(effectUi.icon));
        }
        private void VisualizeEffects()
        {
            foreach (var effect in effectContainer.Effects)
            {
                if (effectObjects.Count > maxVisualizedEffects)
                    return;

                bool skip = false;

                foreach (var effectUi in effectObjects)
                    if (effect == effectUi.effect)
                        skip = true;

                if (skip)
                    continue;

                EffectUiData uiData = effectsUiData.Find(n => n.name == effect.GetType().Name);

                if (uiData == null)
                    continue;

                GameObject effectObj = new GameObject();
                GameObject backgroundEffectObj = new GameObject();

                effectObj.transform.parent = effectsParent;
                backgroundEffectObj.transform.parent = effectsParent;
                backgroundEffectObj.transform.parent = effectObj.transform;

                Image effectIcon = effectObj.AddComponent<Image>();
                effectIcon.fillMethod = Image.FillMethod.Horizontal;
                effectIcon.type = Image.Type.Filled;
                effectIcon.sprite = uiData.Icon;

                Image backgroundEffectIcon = backgroundEffectObj.AddComponent<Image>();
                backgroundEffectIcon.color = backgroundEffectColor;
                backgroundEffectIcon.sprite = uiData.Icon;

                effectObj.AddComponent<CanvasGroup>();

                effectObjects.Add(new DynamicEffectUiData(effectIcon, effect));
            }
        }
    }
}