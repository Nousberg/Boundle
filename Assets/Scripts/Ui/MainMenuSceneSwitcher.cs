using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    [RequireComponent(typeof(SceneSwitcher))]
    public class MainMenuSceneSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject loadingBar;
        [SerializeField] private Image progress;
        [SerializeField] private GameObject menuBar;

        private SceneSwitcher sceneSwitcher => GetComponent<SceneSwitcher>();

        private void Update()
        {
            if (loadingBar.activeInHierarchy)
                progress.fillAmount = (int)SceneSwitcher.CurrentSceneLoadingProgress;
        }

        public void LoadScene(int id)
        {
            if (sceneSwitcher.SwitchScene(id))
            {
                loadingBar.SetActive(true);
                menuBar.SetActive(false);
            }
        }
    }
}