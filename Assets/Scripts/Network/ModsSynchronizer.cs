using Assets.Scripts.Core.Mods;
using System;
using UnityEngine;

namespace Assets.Scripts.Network
{
    [RequireComponent(typeof(ModsLoader))]
    public abstract class ModsSynchronizer : MonoBehaviour
    {
        public event Action<string> OnOploaded;
        public event Action<string> OnDownloaded;
        protected ModsLoader mLoader => GetComponent<ModsLoader>();

        public abstract void Upload(string modPath);
        public abstract void Download(string modUrl, string outputPath);

        protected void UploadEvent(string url) => OnOploaded?.Invoke(url);
        protected void DownloadEvent(string url) => OnDownloaded?.Invoke(url);
    }
}
