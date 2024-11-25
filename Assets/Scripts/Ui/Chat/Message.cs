using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Chat
{
    public class Message : MonoBehaviour
    {
        [field: Header("References")]
        [field: SerializeField] public TextMeshProUGUI AuthorText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI MessageText { get; private set; }
        [field: SerializeField] public Image Background { get; private set; }

        [Header("Properties")]
        [SerializeField] private Ease animationEase;
        [SerializeField] private float animationDuration;

        [Header("Layout Settings")]
        [SerializeField] private float heightPadding;
        [SerializeField] private float widthPadding;
        [SerializeField] private float YAuthorTextOffset;
        [SerializeField] private float XauthorTextOffset;

        private void Start()
        {
            AuthorText.ForceMeshUpdate();
            MessageText.ForceMeshUpdate();

            Bounds messageBounds = MessageText.textBounds;

            Vector2 targetSize = Background.rectTransform.sizeDelta;
            targetSize.x = messageBounds.size.x + widthPadding;
            targetSize.y = messageBounds.size.y + heightPadding;

            Background.rectTransform.sizeDelta = targetSize;

            Vector2 authorPos = AuthorText.rectTransform.anchoredPosition;
            authorPos.x = -targetSize.x / 2f + XauthorTextOffset;
            authorPos.y = targetSize.y / 2f - YAuthorTextOffset;

            AuthorText.rectTransform.anchoredPosition = authorPos;

            Color targetAuthorColor = AuthorText.color;
            Color targetMessageColor = MessageText.color;
            Color targetBackgroundColor = Background.color;

            AuthorText.color = Color.clear;
            MessageText.color = Color.clear;
            Background.color = Color.clear;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(MessageText
                .DOColor(targetMessageColor, animationDuration / 2f)
                .SetEase(animationEase))
                .SetUpdate(UpdateType.Normal, true);

            sequence.Join(Background
                .DOColor(targetBackgroundColor, animationDuration / 2f)
                .SetEase(animationEase))
                .SetUpdate(UpdateType.Normal, true);

            sequence.Join(AuthorText
                .DOColor(targetAuthorColor, animationDuration / 2f)
                .SetEase(animationEase))
                .SetUpdate(UpdateType.Normal, true);

            sequence.AppendInterval(animationDuration);

            sequence.Append(MessageText
                .DOColor(Color.clear, animationDuration / 2f)
                .SetEase(animationEase))
                .SetUpdate(UpdateType.Normal, true);

            sequence.Join(Background
                .DOColor(Color.clear, animationDuration / 2f)
                .SetEase(animationEase))
                .SetUpdate(UpdateType.Normal, true);

            sequence.Join(AuthorText
                .DOColor(Color.clear, animationDuration / 2f)
                .SetEase(animationEase))
                .SetUpdate(UpdateType.Normal, true);

            sequence.OnComplete(() => Destroy(gameObject));
        }

        private void OnDestroy()
        {
            DOTween.Kill(MessageText);
            DOTween.Kill(Background);
            DOTween.Kill(AuthorText);
        }
    }
}
