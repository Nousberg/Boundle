using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    [RequireComponent(typeof(SceneSwitcher))]
    public class MainMenuSceneSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject loadingBar;
        [SerializeField] private GameObject prevPageButton;
        [SerializeField] private Image progress;

        private SceneSwitcher sceneSwitcher => GetComponent<SceneSwitcher>();

        private void Update()
        {
            if (loadingBar.activeInHierarchy)
                progress.fillAmount = (int)SceneSwitcher.CurrentSceneLoadingProgress;
        }

        public void LoadScene(int id)
        {
            sceneSwitcher.SwitchScene(id);
            prevPageButton.SetActive(false);
            loadingBar.SetActive(true);
        }
    }
}