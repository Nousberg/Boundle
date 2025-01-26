using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    public class SceneTransition : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image loadingBackground;

        [Header("Properties")]
        [SerializeField] private float transitionDuration;
        [SerializeField] private Ease transitionEase;

        private PhotonView view => GetComponent<PhotonView>();

        private void Start()
        {
            if (view.IsMine)
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
}