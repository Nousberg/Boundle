using UnityEngine;
using System.IO;
using System.IO.Compression;
using System;
using System.Collections.Generic;
using Assets.Scripts.Ui.Multiplayer;

namespace Assets.Scripts.Core.Mods
{
    public class ModsLoader : MonoBehaviour
    {
        public const string MODS_FOLDER_PATH = "/Mods";
        public const string MOD_EXTENSION = "bmod";
        private const string MOD_MANIFEST = "manifest.json";

        [SerializeField] private AlertManager alertManager;

        public event Action<ManifestData> OnModLoaded;
        public event Action<string> OnFailureLoad;
        public event Action OnReload;

        public List<string> LoadedMods { get; private set; } = new List<string>();

        public void ReloadMods()
        {
            LoadedMods.Clear();
            LoadMods();

            OnReload.Invoke();
        }
        public void LoadMods()
        {
            if (Directory.Exists(MODS_FOLDER_PATH))
            {
                Directory.CreateDirectory(MODS_FOLDER_PATH);
                return;
            }

            foreach (var file in Directory.GetFiles(MODS_FOLDER_PATH, $"*.{MOD_EXTENSION}"))
                LoadModNative(file);
        }
        public void LoadModByPath(string path)
        {
            if (Directory.Exists(path))
                if (Path.GetExtension(path) == MOD_EXTENSION)
                    LoadModNative(path);
        }
        public bool LoadModNative(string path, bool onlyCheck = false)
        {
            ZipArchive archive = ZipFile.OpenRead(path);

            if (archive != null)
            {
                try
                {
                    ManifestData manifest = null;
                    string entryPoint = null;

                    foreach (var entry in archive.Entries)
                    {
                        //manifest check
                        if (entry.Name == MOD_MANIFEST)
                        {
                            try
                            {
                                manifest = JsonUtility.FromJson<ManifestData>(File.ReadAllText(entry.FullName));
                            }
                            catch
                            {
                                throw new Exception("Invalid manifest");
                            }
                        }

                        //entry point check
                        if (entry.FullName == manifest.entryPoint)
                            entryPoint = File.ReadAllText(entry.FullName);
                    }

                    if (entryPoint == null)
                        throw new Exception("Unable to find entry point");

                    if (!onlyCheck)
                    {
                        LoadedMods.Add(entryPoint);
                        OnModLoaded?.Invoke(manifest);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    if (!onlyCheck)
                        OnFailureLoad?.Invoke(e.Message);
                }
            }

            return false;
        }
    }
}