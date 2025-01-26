using System;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public abstract class BannerDownloader : MonoBehaviour
    {
        public event Action<Sprite> OnDownloaded;

        public abstract void Download(string url);

        protected void Downloaded(Sprite s) => OnDownloaded?.Invoke(s);
    }
}
