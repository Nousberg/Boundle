using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Saving
{
    public static class JsonSaver
    {
        public static List<string> Saves { get; private set; } = new List<string>();

        private static string savePath => "Saves/";
        private static List<char> disallowedSymbols = new List<char>() { '#', '@', '!', '*', '&', '%', '$', '^' };

        public static bool Save(object data, string name)
        {
            foreach (char c in name.ToCharArray())
            {
                if (disallowedSymbols.Contains(c))
                    return false;
            }

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            string json = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

            string fullPath = Path.Combine(savePath, name);
            File.WriteAllText(fullPath, json);
            Saves.Add(name);
            return true;
        }
        public static T Load<T>(string name)
        {
            string fullPath = Path.Combine(savePath, name);

            if (!File.Exists(fullPath))
                return default;

            string json = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<T>(json,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
        }
    }
}