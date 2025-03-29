using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Ui.Chat
{
    public class MessageUiData : MonoBehaviour
    {
        [field: Header("References")]
        [field: SerializeField] public TextMeshProUGUI ContentText { get; private set; }
        [field: SerializeField] public CanvasGroup Content { get; private set; }

        [Header("Properties")]
        [SerializeField] private Ease animationEase;
        [SerializeField] private float animationDuration;

        public event Action OnDestroyEvent;

        public string Id { get; private set; }

        public void Init(string id, bool animated)
        {
            Id = id;

            if (!animated)
                return;

            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(animationDuration)
                .Append(Content
                    .DOFade(0f, animationDuration / 4f));

            sequence.OnComplete(() => {
                OnDestroyEvent?.Invoke();
                Destroy(gameObject);
            } )
                .SetEase(animationEase)
                .SetUpdate(UpdateType.Normal, true);
        }
    }
}
