using Assets.Scripts.Ui.Player;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Core.Sound
{
    public class SettingsInitializer : MonoBehaviour
    {
        public static string SettingsSavePath => Path.Combine(Application.persistentDataPath, "settings.json");

        [SerializeField] private AudioMixer audioController;

        private void Start()
        {
            if (File.Exists(SettingsSavePath))
                try
                {
                    DataContainer.settings = JsonUtility.FromJson<SettingsPreset>(File.ReadAllText(SettingsSavePath));
                    Debug.Log(SettingsSavePath);
                }
                catch 
                {
                    InitPreset();
                }
            else
                InitPreset();

            audioController.SetFloat("sfx", Mathf.Lerp(-80f, 0f, DataContainer.settings.sfxVol / 100f));
            audioController.SetFloat("music", Mathf.Lerp(-80f, 0f, DataContainer.settings.musicVol / 100f));
            audioController.SetFloat("master", Mathf.Lerp(-80f, 0f, DataContainer.settings.masterVol / 100f));
            audioController.SetFloat("environment", Mathf.Lerp(-80f, 0f, DataContainer.settings.environmentVol / 100f));
        }

        private void InitPreset()
        {
            SettingsPreset preset = new SettingsPreset();
            preset.resolution = 100;
            preset.quality = 2;
            preset.masterVol = 100f;
            preset.musicVol = 75f;
            preset.environmentVol = 100f;
            preset.sfxVol = 100f;

            DataContainer.settings = preset;
        }
    }
}
