using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Multiplayer
{
    public class Alert : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private CanvasGroup group;
        [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
        [field: SerializeField] public Image Progress { get; private set; }

        [Header("Properties")]
        [SerializeField] private float loadingLerp;
        [SerializeField] private float dissaspearDuration;
        [SerializeField] private float initDuration;
        [SerializeField] private Vector3 dissaspearOffset;
        [SerializeField] private Vector3 initOffset;
        [SerializeField] private Ease animEase;

        public void OnShown(float duration)
        {
            Sequence sequence = DOTween.Sequence();

            rect.localPosition = initOffset;
            group.alpha = 0f;

            sequence.Append(rect.DOLocalMove(Vector3.zero, initDuration)).Join(group.DOFade(1f, initDuration));
            sequence.AppendInterval(duration);
            sequence.Append(group.DOFade(0f, dissaspearDuration));
            sequence.Join(rect.DOLocalMove(dissaspearOffset, dissaspearDuration));
            sequence.SetEase(animEase).SetUpdate(UpdateType.Normal).OnComplete(() => Destroy(gameObject));
        }
        public void SetLoadingProgress(float progress)
        {
            StopCoroutine(nameof(LerpLoading));
            StartCoroutine(LerpLoading(progress));
        }

        private IEnumerator LerpLoading(float val)
        {
            while (val / Progress.fillAmount < 0.98f)
            {
                val = Mathf.Lerp(Progress.fillAmount, val, loadingLerp * Time.deltaTime);
                yield return null;
            }
        }
        private void OnDestroy() => DOTween.Kill(gameObject);
    }
}
