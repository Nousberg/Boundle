using UnityEngine;

namespace Assets.Scripts.Core.Mods
{
    [RequireComponent(typeof(ModsLoader))]
    public class ModImportRequesterFromAppContextMenu : MonoBehaviour
    {
        private ModsLoader mods => GetComponent<ModsLoader>();

        private void Start()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = activity.Call<AndroidJavaObject>("getIntent");

            AndroidJavaObject uri = intent.Call<AndroidJavaObject>("getData");

            if (uri != null)
                mods.LoadModByPath(uri.Call<string>("getPath"));
        }
    }
}