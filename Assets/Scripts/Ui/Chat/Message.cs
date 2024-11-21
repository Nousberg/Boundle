using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Chat
{
    public class Message : MonoBehaviour
    {
        [field: Header("References")]
        [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
        [field: SerializeField] public Image Background { get; private set; }

        [Header("Properties")]
        [SerializeField] private Ease animationEase;
        [SerializeField] private float animationDuration;

        private void Start()
        {
            Color targetTextColor = Text.color;
            Color targetBackgroundColor = Background.color;

            Text.color = Color.clear;
            Background.color = Color.clear;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(Text.DOColor(targetTextColor, animationDuration / 2f)
                .SetEase(animationEase));

            sequence.Join(Background.DOColor(targetBackgroundColor, animationDuration / 2f)
                .SetEase(animationEase));

            sequence.AppendInterval(animationDuration);

            sequence.Append(Text.DOColor(Color.clear, animationDuration / 2f)
                .SetEase(animationEase));

            sequence.Join(Background.DOColor(Color.clear, animationDuration / 2f)
                .SetEase(animationEase));

            sequence.OnComplete(() => Destroy(gameObject));
        }
        private void OnDestroy()
        {
            DOTween.Kill(Text);
            DOTween.Kill(Background);
        }
    }
}
