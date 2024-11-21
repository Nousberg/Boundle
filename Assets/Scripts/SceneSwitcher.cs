using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class SceneSwitcher : MonoBehaviour
    {
        public static float CurrentSceneLoadingProgress 
        { 
            get
            {
                if (loadScene != null)
                    return loadScene.progress;

                return -1f;
            } 
        }

        private static AsyncOperation loadScene;

        public bool SwitchScene(int id)
        {
            if (SceneManager.sceneCountInBuildSettings > id && id > 0)
            {
                loadScene = SceneManager.LoadSceneAsync(id);
                return true;
            }
            return false;
        }
        public void CloseAllScenes()
        {
            Application.Quit();
        }
    }
}
