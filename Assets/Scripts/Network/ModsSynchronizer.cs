using Assets.Scripts.Core.Mods;
using UnityEngine;

namespace Assets.Scripts.Network
{
    [RequireComponent(typeof(ModsLoader))]
    public abstract class ModsSynchronizer : MonoBehaviour
    {
        protected ModsLoader mLoader => GetComponent<ModsLoader>();

        public abstract string Upload(string modPath);
        public abstract void Download(string modUrl, string outputPath);
    }
}
