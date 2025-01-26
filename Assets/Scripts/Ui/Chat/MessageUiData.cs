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

        public event Action<MessageUiData> OnDestroyEvent;

        public Guid Id { get; private set; }

        public void Init(Guid id)
        {
            Id = id;

            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(animationDuration / 2f);

            sequence.Join(Content
                .DOFade(0f, animationDuration / 8f));

            sequence.OnComplete(() => {

                DOTween.Kill(sequence);

                OnDestroyEvent?.Invoke(this);
                Destroy(gameObject);
            } )
                    .SetEase(animationEase)
                    .SetUpdate(UpdateType.Normal, true);
        }
    }
}
