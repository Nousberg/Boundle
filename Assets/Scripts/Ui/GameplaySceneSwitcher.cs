using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    public class GameplaySceneSwitcher : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image loadingBackground;

        [Header("Properties")]
        [SerializeField] private float transitionDuration;
        [SerializeField] private Ease transitionEase;

        private void Start()
        {
            Color targetColor = loadingBackground.color;
            targetColor.a = 0f;

            loadingBackground
                .DOColor(targetColor, transitionDuration)
                .SetUpdate(UpdateType.Normal, true)
                .SetEase(transitionEase)
                .OnComplete(() => loadingBackground.gameObject.SetActive(false));
        }
    }
}