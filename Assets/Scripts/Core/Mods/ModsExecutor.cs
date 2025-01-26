using Jint;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts.Core.Mods
{
    [RequireComponent(typeof(ModsLoader))]
    [RequireComponent(typeof(SceneApi))]
    public class ModsExecutor : MonoBehaviour
    {
        private ModsLoader loader => GetComponent<ModsLoader>();
        private SceneApi api => GetComponent<SceneApi>();

        private Engine jsEngine;

        private void Start()
        {
            loader.OnReload += HandleReload;

            jsEngine = new Engine();

            if (PhotonNetwork.IsMasterClient)
                Execute();
        }
        private void HandleReload()
        {
            jsEngine = new Engine();
            Execute();
        }
        private void Execute() => loader.LoadedMods.ForEach(mod =>
        {
            jsEngine.SetValue("scene", api);
            jsEngine.Execute(mod);
        });
    }
}