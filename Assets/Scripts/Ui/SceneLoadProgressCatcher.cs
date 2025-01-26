using Assets.Scripts.Network;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    [RequireComponent(typeof(MenuSwitcher))]
    public class SceneLoadProgressCatcher : MonoBehaviour
    {
        [SerializeField] private Connector connector;
        [SerializeField] private GameObject progressBarParent;
        [SerializeField] private Image progress;

        private MenuSwitcher mSwitcher => GetComponent<MenuSwitcher>();

        private void Start()
        {
            connector.OnRoomCreation += ShowProgress;
        }
        private void ShowProgress()
        {
            progressBarParent.SetActive(true);
            mSwitcher.DeactivateAll();
        }
        private void Update()
        {
            if (progressBarParent.activeInHierarchy)
                progress.fillAmount = PhotonNetwork.LevelLoadingProgress;
        }
    }
}
